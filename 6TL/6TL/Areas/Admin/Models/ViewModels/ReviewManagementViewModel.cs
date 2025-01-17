using System.Collections.Generic;
using _6TL.Models;

namespace _6TL.ViewModels
{
	public class ReviewManagementViewModel
	{
		public IEnumerable<Product> Products { get; set; }
		public IEnumerable<Review> Reviews { get; set; }
	}
}