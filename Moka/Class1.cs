using System;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Collections.Specialized;

namespace Moka
{
    /// <summary>
    /// Submits post data to a url.
    /// </summary>
    public class PostSubmitter
    {
        /// <summary>
        /// determines what type of post to perform.
        /// </summary>
        /// 
        public string m_parameters;

        public enum PostTypeEnum
        {
            /// <summary>
            /// Does a get against the source.
            /// </summary>
            Get,
            /// <summary>
            /// Does a post against the source.
            /// </summary>
            Post
        }

        private string m_url = string.Empty;
       
        private PostTypeEnum m_type = PostTypeEnum.Get;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostSubmitter()
        {
        }

        /// <summary>
        /// Constructor that accepts a url as a parameter
        /// </summary>
        /// <param name="url">The url where the post will be submitted to.</param>
        public PostSubmitter(string url)
            : this()
        {
            m_url = url;
        }


        /// <summary>
        /// Gets or sets the url to submit the post to.
        /// </summary>
        public string Url
        {
            get
            {
                return m_url;
            }
            set
            {
                m_url = value;
            }
        }


        /// <summary>
        /// Gets or sets the type of action to perform against the url.
        /// </summary>
        public PostTypeEnum Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <returns>a string containing the result of the post.</returns>
        public string Post()
        {
            string result = PostData(m_url, m_parameters);
            return result;
        }
        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to post to.</param>
        /// <returns>a string containing the result of the post.</returns>
        public string Post(string url)
        {
            m_url = url;
            return this.Post();
        }


        /// <summary>
        /// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
        /// </summary>
        /// <param name="postData">The data to post.</param>
        /// <param name="url">the url to post to.</param>
        /// <returns>Returns the result of the post.</returns>
        private string PostData(string url, string postData)
        {
            HttpWebRequest request = null;
            if (m_type == PostTypeEnum.Post)
            {
                Uri uri = new Uri(url);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                request.CookieContainer = new CookieContainer();

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                using (Stream writeStream = request.GetRequestStream())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(postData);
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                Uri uri = new Uri(url + "?" + postData);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
            }
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            return result;
        }
    }
}