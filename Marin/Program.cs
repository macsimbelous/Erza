using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using OpenCvSharp.ImgHash;
using System.Data.SQLite;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
namespace Marin
{
    internal class Program
    {
        private const int ImageSize = 448; // MOAT Tagger uses 448x448 input size
        private const float Threshold = 0.35f; // Standard F1 threshold recommendation
        private static List<ImageInfo> images;
        static void Main(string[] args)
        {
            //Считываем записи
            images = new List<ImageInfo>();
            using (SQLiteConnection Connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                Connection.Open();
                long count_rows;
                //Определяем число записей
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT count(*) FROM images WHERE deleted = 0 AND tags IS NULL";
                    command.Connection = Connection;
                    count_rows = System.Convert.ToInt64(command.ExecuteScalar());
                }
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT image_id, file_path FROM images WHERE deleted = 0 AND tags IS NULL";
                    command.Connection = Connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        ImageInfo img = new ImageInfo(reader.GetInt64(0), reader.GetString(1), new List<(string Tag, float Confidence, int Category)>());

                        images.Add(img);
                        count++;
                        Console.Write("Считано: {0} из {1}\r", count, count_rows);
                    }
                    Console.WriteLine();
                }
            }
            //Проверяем количество
            if (images.Count <= 0)
            {
                Console.WriteLine("Нет изображений без тегов.");
                return;
            }
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            var tags = LoadTags(@"E:\Downloads\selected_tags (1).csv");
            int gpuDeviceId = 0; // The GPU device ID to execute on
            using var gpuSessionOptoins = SessionOptions.MakeSessionOptionWithCudaProvider(gpuDeviceId);
            using var session = new InferenceSession(@"E:\Downloads\model (1).onnx", gpuSessionOptoins);
            for (int i = 0; i < images.Count; i++)
            {
                try
                {
                    Console.Write($"{i + 1}/{images.Count} {images[i].FilePath} ");
                    //var predictedTags = GetTags(@"E:\Downloads\model (1).onnx", @"E:\Downloads\selected_tags (1).csv", @"F:\pixiv_all\147319203_p0.png");
                    float[] inputData = PreprocessImage(images[i].FilePath);

                    // 3. Prepare the ONNX Tensor Input
                    var inputMeta = new List<NamedOnnxValue>
                    {
                        //NamedOnnxValue.CreateFromTensor("input_1:0", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
                        NamedOnnxValue.CreateFromTensor("input", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
                    };
                    using var results = session.Run(inputMeta);

                    // Get output data (typically named "output_1" or the first item)
                    var outputTensor = results.First().AsTensor<float>();
                    float[] probabilities = outputTensor.ToArray();

                    // 5. Match results with tags and filter by threshold
                    var predictedTags = new List<(string Tag, float Confidence, int Category)>();

                    for (int i2 = 0; i2 < probabilities.Length; i2++)
                    {
                        if (probabilities[i2] >= Threshold && i2 < tags.Count)
                        {
                            predictedTags.Add((tags[i2].Name, probabilities[i2], tags[i2].Category));
                        }
                    }
                    Console.Write($" {predictedTags.Count}");
                    Console.WriteLine($"...OK");
                    images[i].Tags.AddRange(predictedTags);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            End();
        }
        private static List<(string Tag, float Confidence, int Category)> GetTags(string modelPath, string csvPath, string imagePath)
        {
            // 1. Load the tag names mapping from the CSV file
            var tags = LoadTags(csvPath);

            // 2. Preprocess the image
            float[] inputData = PreprocessImage(imagePath);

            // 3. Prepare the ONNX Tensor Input
            var inputMeta = new List<NamedOnnxValue>
            {
                //NamedOnnxValue.CreateFromTensor("input_1:0", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
                NamedOnnxValue.CreateFromTensor("input", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
            };

            // 4. Run Inference via ONNX Runtime
            //using var session = new InferenceSession(modelPath);
            int gpuDeviceId = 0; // The GPU device ID to execute on
            using var gpuSessionOptoins = SessionOptions.MakeSessionOptionWithCudaProvider(gpuDeviceId);
            using var session = new InferenceSession(modelPath, gpuSessionOptoins);

            /*foreach (var input in session.InputMetadata)
            {
                Console.WriteLine($"Expected Input Name: {input.Key}");
            }*/
            using var results = session.Run(inputMeta);

            // Get output data (typically named "output_1" or the first item)
            var outputTensor = results.First().AsTensor<float>();
            float[] probabilities = outputTensor.ToArray();

            // 5. Match results with tags and filter by threshold
            var predictedTags = new List<(string Tag, float Confidence, int Category)>();

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] >= Threshold && i < tags.Count)
                {
                    predictedTags.Add((tags[i].Name, probabilities[i], tags[i].Category));
                }
            }
            return predictedTags;
        }
        private static float[] PreprocessImage(string path)
        {
            // Load using OpenCvSharp
            using Mat src = Cv2.ImRead(path, ImreadModes.Color);
            if (src.Empty()) throw new Exception("Could not load image.");

            // Resize to 448x448
            using Mat resized = new Mat();
            Cv2.Resize(src, resized, new Size(ImageSize, ImageSize), 0, 0, InterpolationFlags.Cubic);

            // Convert BGR (OpenCV Default) to RGB
            using Mat rgb = new Mat();
            Cv2.CvtColor(resized, rgb, ColorConversionCodes.BGR2RGB);

            // Convert byte array to float array normalized to 0.0f - 255.0f (WD models don't use 0-1 scaling)
            // Format: NHWC -> [1, 448, 448, 3]
            float[] floatBuffer = new float[ImageSize * ImageSize * 3];
            int index = 0;

            for (int y = 0; y < ImageSize; y++)
            {
                for (int x = 0; x < ImageSize; x++)
                {
                    Vec3b pixel = rgb.At<Vec3b>(y, x);
                    floatBuffer[index++] = pixel.Item0; // R
                    floatBuffer[index++] = pixel.Item1; // G
                    floatBuffer[index++] = pixel.Item2; // B
                }
            }

            return floatBuffer;
        }

        private static List<(string Name, int Category)> LoadTags(string csvPath)
        {
            var tagsList = new List<(string Name, int Category)>();
            var lines = System.IO.File.ReadAllLines(csvPath);

            // Skip the header (tag_id, name, category, count)
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Simple splitting logic. Be aware some tags may contain commas, but Danbooru tags use underscores.
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    string name = parts[1].Trim('"');
                    int category = int.Parse(parts[2]);
                    tagsList.Add((name, category));
                }
            }
            return tagsList;
        }
        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            End();
            Console.WriteLine("\nExit");
            Environment.Exit(0);
        }
        static void End()
        {
            int count_add = 0;
            //Добавляем теги в БД
            using (SQLiteConnection Connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                Connection.Open();
                SQLiteTransaction transact = Connection.BeginTransaction();
                for (int i = 0; i < images.Count; i++)
                {
                    if (images[i].Tags.Count > 0)
                    {
                        Console.Write($"{i + 1}/{images.Count} {images[i].ImageID} {images[i].Tags.Count}");
                        List<long> tag_ids = new List<long>();
                        foreach (var tag in images[i].Tags)
                        {
                            long id = GetTagID(tag.Tag, Connection);
                            if (id < 0)
                            {
                                AddTag(tag.Tag, Connection);
                                id = GetTagID(tag.Tag, Connection);
                            }
                            tag_ids.Add(id);
                        }
                        AddTagsToImage(images[i].ImageID, tag_ids, Connection);
                        count_add++;
                        Console.WriteLine($"...OK");
                    }
                }
                transact.Commit();
            }
            Console.WriteLine($"Добавлены теги для {count_add} изображений");
        }
        static void AddTagsToImage(long ImageID, IEnumerable<long> TagIDs, SQLiteConnection Connection)
        {
            List<long> tagids = new List<long>();
            foreach (long TagID in TagIDs)
            {
                tagids.Add(TagID);
            }
            using (SQLiteCommand command = new SQLiteCommand("SELECT tags FROM images WHERE image_id = @image_id", Connection))
            {
                command.Parameters.AddWithValue("image_id", ImageID);
                object o = command.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    tagids.AddRange(ParseStringOfTagIDs((string)o));
                }
            }
            tagids = tagids.Distinct().ToList();
            using (SQLiteCommand command = new SQLiteCommand("UPDATE images SET tags = @tags WHERE image_id = @image_id", Connection))
            {
                command.Parameters.AddWithValue("tags", GetStringOfTagIDs(tagids));
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
            }
        }
        static string GetStringOfTagIDs(IEnumerable<long> tags)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#");
            foreach (long tag_id in tags)
            {
                sb.Append(tag_id.ToString());
                sb.Append("#");
            }
            return sb.ToString();
        }
        static List<long> ParseStringOfTagIDs(string StringTagIDs)
        {
            List<long> tag_ids = new List<long>();
            string[] tags = StringTagIDs.Split('#');
            foreach (string tag_id in tags)
            {
                try
                {
                    tag_ids.Add(long.Parse(tag_id));
                }
                catch (Exception) { }
            }
            return tag_ids;
        }
        static long GetImageID(string? Hash, SQLiteConnection Connection)
        {
            string sql = "SELECT image_id FROM images WHERE hash = @hash";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("hash", Hash);
                object o = command.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                {
                    return -1;
                }
                else
                {
                    return Convert.ToInt64(o);
                }
            }
        }
        static long GetTagID(string Tag, SQLiteConnection Connection)
        {
            string sql = "SELECT tag_id FROM tags WHERE tag = @tag";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                object o = command.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                {
                    return -1;
                }
                else
                {
                    return System.Convert.ToInt64(o);
                }
            }
        }
        static void AddTag(string Tag, SQLiteConnection Connection)
        {
            string sql = "INSERT INTO tags (tag) VALUES (@tag);";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                command.ExecuteNonQuery();
            }
        }
    }
    internal class ImageInfo
    {
        public long ImageID;
        public string FilePath;
        public List<(string Tag, float Confidence, int Category)> Tags;
        public ImageInfo(long ImageID, string FilePath, List<(string Tag, float Confidence, int Category)> Tags) 
        { 
            this.ImageID = ImageID;
            this.FilePath = FilePath;
            this.Tags = Tags;
        }
    }
}

