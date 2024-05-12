using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pharmacy.Areas.Admin.Service
{
	
	public class AWS_S3Controller : Controller
	{
		private readonly IAmazonS3 _s3Client;

		public AWS_S3Controller(IAmazonS3 s3Client)
		{
			_s3Client = s3Client;
		}

		[HttpPost("upload")]
		public async Task<IActionResult> UploadFileAsync(IFormFile file)
		{
			string key = Guid.NewGuid().ToString();
			var request = new PutObjectRequest()
			{
				BucketName = "pharmacynetcore",
				Key = key,
				InputStream = file.OpenReadStream(),
				ContentType = "image/jpeg"
			};

			PutObjectResponse putObjectResponse = await _s3Client.PutObjectAsync(request);

			Console.WriteLine(putObjectResponse);

			var preSignedUrlRequest = new GetPreSignedUrlRequest
			{
				BucketName = "pharmacynetcore",
				Key = key,
				Expires = DateTime.Today.AddDays(1)
			};

			string preSignedUrl = _s3Client.GetPreSignedURL(preSignedUrlRequest);
			return Ok(preSignedUrl );
		}
	}
}
