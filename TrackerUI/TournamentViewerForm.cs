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
	public partial class TournamentViewerForm : Form
	{
		private TournamentModel tournament;
		List<int> rounds = new List<int>();
		List<MatchupModel> selectedMatchups = new List<MatchupModel>();

		public TournamentViewerForm(TournamentModel tournamentModel)
		{
			InitializeComponent();

			this.tournament = tournamentModel;

			LoadFormData();

			LoadRounds();
		}

		private void LoadFormData()
		{
			tournamentName.Text = tournament.TournamentName;

		}

		private void WireUpRoundLists()
		{
			roundDropDown.DataSource = null;
			roundDropDown.DataSource = rounds;
		}

		private void WireUpMatchupLists()
		{
			matchupListBox.DataSource = null;
			matchupListBox.DataSource = selectedMatchups;
			matchupListBox.DisplayMember = "DisplayName";
			matchupListBox.Refresh();

			if (matchupListBox.SelectedItem == null && matchupListBox.Items.Count > 0)
			{
				matchupListBox.SetSelected(0, true);
			}
		}

		private void LoadRounds()
		{
			rounds = new List<int>();

			rounds.Add(1);
			int currRound = 1;

			foreach (List<MatchupModel> matchups in tournament.Rounds)
			{
				if (matchups.First().MatchupRound > currRound)
				{
					currRound = matchups.First().MatchupRound;
					rounds.Add(currRound);
				}
			}

			WireUpRoundLists();
		}

		private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadMatchups();
		}

		private void LoadMatchups()
		{
			int round = (int)roundDropDown.SelectedItem;

			foreach (List<MatchupModel> matchups in tournament.Rounds)
			{
				if (matchups.First().MatchupRound == round)
				{
					selectedMatchups.Clear();

					foreach (MatchupModel matchup in matchups)
					{
						if (matchup.Winner == null || !unplayedOnlyCheckbox.Checked)
						{
							selectedMatchups.Add(matchup);
						}
					}
				}
			}

			DisplayMatchupInfo();
		}

		private void DisplayMatchupInfo()
		{
			bool isVisible = (selectedMatchups.Count > 0);

			teamOneName.Visible = isVisible;
			teamOneScoreLabel.Visible = isVisible;
			teamOneScoreValue.Visible = isVisible;
			teamTwoName.Visible = isVisible;
			teamTwoScoreLabel.Visible = isVisible;
			teamTwoScoreValue.Visible = isVisible;
			versusLabel.Visible = isVisible;
			scoreButton.Visible = isVisible;

			WireUpMatchupLists();


			//if (isVisible)
			//{
			//	WireUpMatchupLists();
			//}
		}

		private void LoadMatchup()
		{
			MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

			for (int i = 0; i < m.Entries.Count; i++)
			{
				if (i == 0)
				{
					if (m.Entries[0].TeamCompeting != null)
					{
						teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
						teamOneScoreValue.Text = m.Entries[0].Score.ToString();

						teamTwoName.Text = "<bye>";
						teamTwoScoreValue.Text = "";
					}
					else
					{
						teamOneName.Text = "Not Yet Determined";
						teamOneScoreValue.Text = "";
					}
				}

				if (i == 1)
				{
					if (m.Entries[1].TeamCompeting != null)
					{
						teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
						teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
					}
					else
					{
						teamTwoName.Text = "Not Yet Determined";
						teamTwoScoreValue.Text = "";
					}
				}
			}
		}

		private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (matchupListBox.DataSource != null)
			{
				LoadMatchup();
			}
		}

		private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			LoadMatchups();
			WireUpMatchupLists();
		}

		private void scoreButton_Click(object sender, EventArgs e)
		{
			MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
			double teamOneScore = 0;
			double teamTwoScore = 0;

			if (m.Entries[0].TeamCompeting == null && m.Entries[1].TeamCompeting == null)
			{
				MessageBox.Show("This match is not determined yet.");
				return;
			}

			for (int i = 0; i < m.Entries.Count; i++)
			{
				if (i == 0)
				{
					if (m.Entries[0].TeamCompeting != null)
					{
						bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);

						if (scoreValid)
						{
							m.Entries[0].Score = teamOneScore;
						}
						else
						{
							MessageBox.Show($"Enter valid score for team { m.Entries[0].TeamCompeting.TeamName }.");
							return;
						} 
					}
				}

				if (i == 1)
				{
					if (m.Entries[1].TeamCompeting != null)
					{
						bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

						if (scoreValid)
						{
							m.Entries[1].Score = teamTwoScore;
						}
						else
						{
							MessageBox.Show($"Enter valid score for team { m.Entries[1].TeamCompeting.TeamName }.");
							return;
						}
					}
				}
			}

			if (teamOneScore > teamTwoScore)
			{
				m.Winner = m.Entries[0].TeamCompeting;
			}
			else if (teamTwoScore > teamOneScore)
			{
				m.Winner = m.Entries[1].TeamCompeting;
			}
			else
			{
				MessageBox.Show("I do not handle tie games.");
				return;
			}

			foreach (List<MatchupModel> round in tournament.Rounds)
			{
				foreach (MatchupModel rm in round)
				{
					foreach (MatchupEntryModel me in rm.Entries)
					{
						if (me.ParentMatchup != null)
						{
							if (me.ParentMatchup.Id == m.Id)
							{
								me.TeamCompeting = m.Winner;
								GlobalConfig.Connection.UpdateMatchup(rm);
							} 
						}
					}
				}
			}

			LoadMatchups();
			WireUpMatchupLists();

			GlobalConfig.Connection.UpdateMatchup(m);
		}
	}
}
