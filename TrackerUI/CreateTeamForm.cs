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
		private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();	// loading data from database or text file
		private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
		ITeamRequester whoNeedsNewTeam;

		public CreateTeamForm(ITeamRequester caller)
		{
			this.whoNeedsNewTeam = caller;
			InitializeComponent();

			//Test_CreateSampleData();

			WireUpLists();

		}

		private void Test_CreateSampleData()
		{
			availableTeamMembers.Add(new PersonModel() {
				Id = 1,
				FirstName = "John",
				LastName = "Silver",
				});

			availableTeamMembers.Add(new PersonModel()
			{
				Id = 2,
				FirstName = "Alan",
				LastName = "Calling",
			});

			selectedTeamMembers.Add(new PersonModel()
			{
				Id = 5,
				FirstName = "Olive",
				LastName = "Oil",
			});

			selectedTeamMembers.Add(new PersonModel()
			{
				Id = 2,
				FirstName = "Grand",
				LastName = "Stand",
			});
		}

		private void WireUpLists()
		{
			addMemberDropDown.DataSource = null;

			addMemberDropDown.DataSource = availableTeamMembers;
			addMemberDropDown.DisplayMember = "FullName";

			teamMembersListBox.DataSource = null;

			teamMembersListBox.DataSource = selectedTeamMembers;
			teamMembersListBox.DisplayMember = "FullName";
		}

		private void createMemberButton_Click(object sender, EventArgs e)
		{
			if (ValidateCreateMemberForm())
			{
				var person = new PersonModel();

				person.FirstName = firstNameValue.Text;
				person.LastName = lastNameValue.Text;
				person.EmailAddress = emailValue.Text;
				person.CellphoneNumber = cellphoneValue.Text;

				person = GlobalConfig.Connection.CreatePerson(person);

				selectedTeamMembers.Add(person);
				WireUpLists();

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

		private bool ValidateCreateMemberForm()
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


		private bool ValidateCreateTeamForm()
		{
			bool output = true;

			if (teamNameValue.Text.Length == 0)
				output = false;

			if (selectedTeamMembers.Count == 0)
				output = false;

			return output;
		}

		private void addMemberButton_Click(object sender, EventArgs e)
		{
			if (availableTeamMembers.Count > 0)
			{
				PersonModel p = (PersonModel)addMemberDropDown.SelectedItem;

				availableTeamMembers.Remove(p);
				selectedTeamMembers.Add(p);

				WireUpLists();
			}
		}

		private void removeSelectedMemberButton_Click(object sender, EventArgs e)
		{
			if (selectedTeamMembers.Count > 0 && teamMembersListBox.SelectedItem != null)
			{
				PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

				availableTeamMembers.Add(p);
				selectedTeamMembers.Remove(p);

				WireUpLists();
			}
		}

		private void createTeamButton_Click(object sender, EventArgs e)
		{
			if (ValidateCreateTeamForm())
			{
				TeamModel team = new TeamModel();

				team.TeamName = teamNameValue.Text;
				team.TeamMembers = selectedTeamMembers.ToList();

				GlobalConfig.Connection.CreateTeam(team);
				whoNeedsNewTeam.GiveTeam(team);

				this.Close();
			}
			else
			{
				MessageBox.Show("Type a team name and choose at least one member.");
			}
		}
	}
}
