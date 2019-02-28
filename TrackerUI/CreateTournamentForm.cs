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
	public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
	{
		private List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
		private List<TeamModel> selectedTeams = new List<TeamModel>();
		private List<PrizeModel> selectedPrizes = new List<PrizeModel>();

		public CreateTournamentForm()
		{
			InitializeComponent();

			WireUpLists();
		}

		private void WireUpLists()
		{
			selectTeamDropDown.DataSource = null;

			selectTeamDropDown.DataSource = availableTeams;
			selectTeamDropDown.DisplayMember = "TeamName";

			selectedTeamsListBox.DataSource = null;

			selectedTeamsListBox.DataSource = selectedTeams;
			selectedTeamsListBox.DisplayMember = "TeamName";

			prizesListBox.DataSource = null;

			prizesListBox.DataSource = selectedPrizes;
			prizesListBox.DisplayMember = "PrizeName";
		}

		private void addTeamButton_Click(object sender, EventArgs e)
		{
			TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

			if (t != null)
			{

				selectedTeams.Add(t);
				availableTeams.Remove(t);

				WireUpLists(); 
			}
		}

		private void createPrizeButton_Click(object sender, EventArgs e)
		{
			// Call the create prize form
			CreatePrizeForm frm = new CreatePrizeForm(this);
			frm.Show();

		}

		public void GivePrize(PrizeModel model)
		{
			// get back PrizeModel from the form
			selectedPrizes.Add(model);

			// save prize to selectedPrizes list
			WireUpLists();
		}

		private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			CreateTeamForm frm = new CreateTeamForm(this);
			frm.Show();
		}

		public void GiveTeam(TeamModel model)
		{
			selectedTeams.Add(model);
			WireUpLists();
		}

		private void removeSelectedTeamButton_Click(object sender, EventArgs e)
		{
			TeamModel team = (TeamModel)selectedTeamsListBox.SelectedItem;

			if (team != null)
			{
				availableTeams.Add(team);
				selectedTeams.Remove(team);
				WireUpLists(); 
			}
		}

		private void tournamentPlayersLabel_Click(object sender, EventArgs e)
		{

		}

		private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
		{
			PrizeModel prize = (PrizeModel)prizesListBox.SelectedItem;

			if (prize != null)
			{
				selectedPrizes.Remove(prize);
				WireUpLists();
			}
		}

		private void createTournamentButton_Click(object sender, EventArgs e)
		{
			if (ValidateCreateTournamentForm())
			{
				TournamentModel t = new TournamentModel();

				t.TournamentName = tournamentNameValue.Text;
				t.EntryFee = decimal.Parse(entryFeeValue.Text);
				t.EnteredTeams = selectedTeams;
				t.Prizes = selectedPrizes;
				// TODO - t.Round

				GlobalConfig.Connection.CreateTournament(t);

				this.Close();
			}
			else
			{
				MessageBox.Show("Invalid information in the form.",
					"Invalid data", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);
			}
		}

		private bool ValidateCreateTournamentForm()
		{
			bool output = true;

			if (tournamentNameValue.Text.Length == 0)
				output = false;

			if (entryFeeValue.Text.Length == 0)
				output = false;

			int entryFee = 0;

			bool entryFeeValid = int.TryParse(entryFeeValue.Text, out entryFee);

			if (entryFee < 0)
				output = false;

			if (selectedTeams.Count < 1)
				output = false;

			return output;
		}
	}
}
