using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
	public partial class CreateTeamForm : Form
	{
		public CreateTeamForm()
		{
			InitializeComponent();
		}

		private void createMemberButton_Click(object sender, EventArgs e)
		{
			if (ValidateForm())
			{
				var person = new PersonModel();

				person.FirstName = firstNameValue.Text;
				person.LastName = lastNameValue.Text;
				person.EmailAddress = emailValue.Text;
				person.CellphoneNumber = cellphoneValue.Text;

				GlobalConfig.Connection.CreatePerson(person);

				firstNameValue.Text = "";
				lastNameValue.Text = "";
				emailValue.Text = "";
				cellphoneValue.Text = "";
			}
			else
			{
				MessageBox.Show("One or more fields in Add New Member section has invalid data.");
			}
		}

		private bool ValidateForm()
		{
			if (firstNameValue.Text.Length < 0)
				return false;

			if (lastNameValue.Text.Length < 0)
				return false;

			if (emailValue.Text.Length < 0 || !emailValue.Text.Contains("@"))
				return false;
			
			if (cellphoneValue.Text.Length < 0)
				return false;

			return true;
		}
	}
}
