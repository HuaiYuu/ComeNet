namespace AWSWEBAPP.Models
{
    public class Aws
    {


        public class AWSS3Object
        {
            public string Name { get; set; } = null!;
            public MemoryStream InputStream { get; set; } = null!;
            public string BucketName { get; set; } = null!;
        }

        public class S3ResponseDTO
        {
            public int StatusCode { get; set; } = 200;           
            public string message { get; set; } = "";
        }

        public class AWSCredentials
        {           
            public string AwsKey { get; set; } = "";
            public string AwsSecretKey { get; set; } = "";
        }

        public class Constants
        {
            public static string AccessKey { get; set; } = "AccessKey";
            public string SecretKey { get; set; } = "SecretKey";
        }




		public class MidtermColor
		{
			public string code { get; set; }
			public string name { get; set; }
		}

		public class MidtermItem
		{
			public int id { get; set; }
			public int price { get; set; }
			public MidtermColor color { get; set; }
			public string size { get; set; }
			public int qty { get; set; }
		}

		public class MidtermDataSet
		{
			public int total { get; set; }
			public List<MidtermItem> list { get; set; }
		}

		public class dashbaord
		{
			public int id { get; set; }
			public int productid { get; set; }
			public int price { get; set; }
			public string colorcode { get; set; }
			public string colorname { get; set; }			
			public string size { get; set; }
			public int qty { get; set; }
		}








	}
}
