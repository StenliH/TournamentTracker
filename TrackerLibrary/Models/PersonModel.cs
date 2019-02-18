using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
	public class PersonModel
	{
		/// <summary>
		/// The unique identifier for a person.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The first name of the person.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Tha last name of the person.
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// The email address of the person.
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// The phone number of the person.
		/// </summary>
		public string CellphoneNumber { get; set; }
	}
}
