using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace ICC.Predictor.Blanket.DataPopulation
{
    public class Feed
    {
        private readonly calendar _Calendar;
        private readonly string _League;
        private readonly string _CricketHostedApi;
        private readonly string _Client;
        private static string _FixturesFeed { get { return "type=fixtures"; } }
        private static string _SquadFeed { get { return "type=squad"; } }
        public static Squad_list _SquadList;

        public Feed(IOptions<Application> appSettings, string league, string v_lang_code = "en")
        {
            _CricketHostedApi = appSettings.Value.Admin.Feed.API;
            _Client = $"client={appSettings.Value.Admin.Feed.Client}";
            _League = !string.IsNullOrEmpty(league) ? "name=" + league : "";
            _Calendar = GetCalendar(v_lang_code);
        }

        private calendar GetCalendar(string v_lang_code)
        {
            string url = string.Format("{0}?{1}&{2}&{3}&{4}", _CricketHostedApi, _Client, _FixturesFeed, _League, "lang=" + v_lang_code);
            XmlSerializer xml_serializer = new XmlSerializer(typeof(calendar));
            return (calendar)xml_serializer.Deserialize(XMLHelper.ReadXMLApi(url));
        }

        public Squad_list GetSquadsByURI(string team_ID, string series_id, string v_lang_code, out int optType)
        {
            optType = 1;
            try
            {
                Squad_list _vSquadList = new Squad_list();
                JsonResult obj = new JsonResult();
                XmlSerializer xml_serializer = new XmlSerializer(typeof(Squad_list));
                System.Xml.XmlReader objXMLReader = null;
                string url = string.Format("{0}?{1}&{2}&{3}&{4}&{5}&{6}", _CricketHostedApi, _Client, _SquadFeed, "id=" + team_ID, "seriesid=" + series_id, "lang=" + v_lang_code, "accept=xml");

                objXMLReader = XMLHelper.ReadXMLApi(url, ref obj);

                if (obj.error)
                {
                    JsonResult obj2 = new JsonResult();
                    objXMLReader = GetSquadsByAlternativeURI(team_ID, ref _vSquadList, ref obj2);

                    if (obj2.error || objXMLReader == null)
                    {
                        return new Squad_list();
                    }
                    optType = 2;
                }
                else
                {
                    optType = 1;
                    _vSquadList = (Squad_list)xml_serializer.Deserialize(objXMLReader);
                }

                _SquadList = _vSquadList;
                return _vSquadList;
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private System.Xml.XmlReader GetSquadsByAlternativeURI(string team_ID, ref Squad_list _vSquadList, ref JsonResult obj2)
        {
            System.Xml.XmlReader objXMLReader;

            try
            {
                string url = string.Format("{0}?{1}&{2}&{3}&{4}", _CricketHostedApi, _Client, _SquadFeed, "filename=" + team_ID + "_currentplayer", "accept=xml");
                XmlSerializer xml_serializer = new XmlSerializer(typeof(Squad_list));

                objXMLReader = XMLHelper.ReadXMLApi(url, ref obj2);

                _vSquadList = (Squad_list)xml_serializer.Deserialize(objXMLReader);
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objXMLReader;
        }

        #region " Tournaments "

        public List<SITournament> GetTournaments()
        {
            List<string> tour = _Calendar.match.Where(c => c.tour_Id != null && c.tour_Id != "").OrderBy(c => c.tour_Id).Select(c => c.tour_Id).Distinct().ToList(); // get distinct tournament ids
            List<SITournament> vTournament = new List<SITournament>();

            foreach (var item in tour) // looping on distinct tournament ids
            {
                var Tournament = _Calendar.match.Where(c => c.tour_Id == item).FirstOrDefault(); // match with api list and take first record to prevent duplicate tournament entries                
                DateTime Tournament_Start_Date = GetSeriesByTourId(Tournament.tour_Id).OrderBy(c => c.series_start_date).Select(c => c.series_start_date).FirstOrDefault();
                DateTime Tournament_End_Date = GetSeriesByTourId(Tournament.tour_Id).OrderByDescending(c => c.series_end_date).Select(c => c.series_end_date).FirstOrDefault();
                vTournament.Add(new SITournament()
                {
                    tour_id = Convert.ToInt32(Tournament.tour_Id),
                    tour_name = Tournament.tourname,
                    tour_start_date = Tournament_Start_Date,
                    tour_end_date = Tournament_End_Date
                });
            }
            return vTournament;
        }

        #endregion

        #region " Series "

        public List<SISeries> GetSeries()
        {
            IEnumerable<string> vSeriesId = _Calendar.match.Where(c => c.series_Id != null && c.series_Id != "" && c.tour_Id != null && c.tour_Id != "").Select(c => c.series_Id).Distinct(); // get the list of distinct series ids
            List<SISeries> vSeries = new List<SISeries>();
            foreach (var series_id in vSeriesId) // looping on distinct series ids
            {
                var vSeriesData = _Calendar.match.Where(c => c.series_Id == series_id).FirstOrDefault(); // get the first record of series by series ids to prevent duplicate series entries in BS_SERIES

                if (vSeriesData.tour_Id != null && vSeriesData.tour_Id.Trim() != "" &&
                    vSeriesData.series_Id != null && vSeriesData.series_Id.Trim() != "")
                {
                    vSeries.Add(new SISeries()
                    {
                        tour_id = Convert.ToInt32(vSeriesData.tour_Id),
                        series_Id = Convert.ToInt32(vSeriesData.series_Id),
                        seriesname = vSeriesData.seriesname,
                        series_short_display_name = vSeriesData.series_short_display_name,
                        series_start_date = XMLHelper.ParseExactDate(vSeriesData.series_start_date, "M/d/yyyy", "dd-MM-yyyy"),
                        series_end_date = XMLHelper.ParseExactDate(vSeriesData.series_end_date, "M/d/yyyy", "dd-MM-yyyy"),
                        comp_type = vSeriesData.matchtype
                    });
                }

            }
            return vSeries;
        }

        public List<SISeries> GetSeriesByTourId(string v_TourId)
        {
            List<SISeries> vSeries = _Calendar.match.Where(c => c.tour_Id == v_TourId).Select(d => new SISeries
            {
                tour_id = Convert.ToInt32(d.tour_Id),
                series_Id = Convert.ToInt32(d.series_Id),
                seriesname = d.seriesname,
                series_short_display_name = d.series_short_display_name,
                series_start_date = XMLHelper.ParseExactDate(d.series_start_date, "M/d/yyyy", "dd-MM-yyyy"),
                series_end_date = XMLHelper.ParseExactDate(d.series_end_date, "M/d/yyyy", "dd-MM-yyyy"),
            }).Distinct().ToList();

            return vSeries;
        }

        #endregion

        #region " Fixtures "

        public IList<SIMatch> GetMatches(string v_Tour_Id, string v_Series_Id)
        {
            var vMatch = _Calendar.match.Where(c => c.tour_Id == v_Tour_Id && c.series_Id == v_Series_Id).ToList();
            IList<SIMatch> vSIMatch = vMatch.Select(c => new SIMatch
            {
                tour_id = Convert.ToInt32(c.match_Id),
                series_id = Convert.ToInt32(c.series_Id),
                match_Id = Convert.ToInt32(c.match_Id),
                home_team_id = c.teama_Id == "" ? 0 : Convert.ToInt32(c.teama_Id),
                home_team_name = c.teama,
                home_team_short_name = c.teama_short,
                away_team_id = c.teamb_Id == "" ? 0 : Convert.ToInt32(c.teamb_Id),
                away_team_name = c.teamb,
                away_team_short_name = c.teamb_short,
                matchdate_ist = XMLHelper.ParseExactDate(c.matchdate_ist + " " + c.matchtime_ist, "M/d/yyyy HH:mm", "dd-MM-yyyy HH:mm"),
                match_name = c.teama + " vs " + c.teamb,
                match_file = c.matchfile,
                match_number = c.matchnumber,
                match_result = c.matchresult,
                match_status = c.matchstatus,
                match_time_gmt = c.matchtime_gmt,
                match_time_ist = c.matchtime_ist,
                match_time_local = c.matchtime_local,
                match_type = c.matchtype,
                venue = c.venue
            }).ToList();

            return vSIMatch;
        }

        #endregion

        #region " Teams "

        public IList<SITeam> GetTeams(string v_Tournament_Id, string v_Series_Id)
        {
            try
            {
                var vMatch = _Calendar.match.Where(c => c.tour_Id == v_Tournament_Id && c.series_Id == v_Series_Id).ToList();
                List<string> TeamAIds = vMatch.Where(i => i.teama_Id != null && i.teama_Id.Trim() != "").Select(c => c.teama_Id).Distinct().ToList();
                List<string> TeamBIds = vMatch.Where(i => i.teamb_Id != null && i.teamb_Id.Trim() != "").Select(c => c.teamb_Id).Distinct().ToList();
                List<string> FinalTeamIds = TeamAIds.Union(TeamBIds).Distinct().ToList();
                IList<SITeam> vSITeam = new List<SITeam>();

                foreach (var item in FinalTeamIds)
                {
                    var TeamAData = _Calendar.match.Where(c => c.teama_Id == item).FirstOrDefault();
                    var TeamBData = _Calendar.match.Where(c => c.teamb_Id == item).FirstOrDefault();
                    if (TeamAData != null)
                    {
                        vSITeam.Add(new SITeam()
                        {
                            tour_id = Convert.ToInt32(TeamAData.match_Id),
                            series_id = Convert.ToInt32(TeamAData.series_Id),
                            team_id = TeamAData.teama_Id == "" ? 0 : Convert.ToInt32(TeamAData.teama_Id),
                            team_name = TeamAData.teama,
                            team_short = TeamAData.teama_short,

                        });
                    }
                    else if (TeamBData != null)
                    {
                        vSITeam.Add(new SITeam()
                        {
                            tour_id = Convert.ToInt32(TeamBData.match_Id),
                            series_id = Convert.ToInt32(TeamBData.series_Id),
                            team_id = TeamBData.teamb_Id == "" ? 0 : Convert.ToInt32(TeamBData.teamb_Id),
                            team_name = TeamBData.teamb,
                            team_short = TeamBData.teamb_short,
                        });
                    }
                }
                return vSITeam;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region " Players "

        public IList<SIPlayer> GetPlayers(string v_Team_Id, string v_Tour_Id, string v_Series_Id, string v_lang_code, out int optType)
        {
            try
            {
                IList<SIPlayer> vPLayer = null;

                Squad_list ObjPlayers = GetSquadsByURI(v_Team_Id, v_Series_Id, v_lang_code, out optType);

                if (ObjPlayers == null)
                    return vPLayer;

                vPLayer = ObjPlayers.Players.Player.Select(c => new SIPlayer()
                {
                    tour_id = Convert.ToInt32(v_Tour_Id),
                    series_id = Convert.ToInt32(v_Series_Id),
                    playerid = Convert.ToInt32(c.Id),
                    player_name = c.Text,
                    player_display_name = c.Text,
                    skill_name = c.Skill,
                    team_id = Convert.ToInt32(ObjPlayers.Team_id)
                }).ToList();

                //PlayerSkills obj = new PlayerSkills();

                //foreach (SIPlayer player in vPLayer)
                //{
                //    if (player.skill_name != "")
                //    {
                //        if (obj.skillsList.FindIndex(a => a.ToLower() == player.skill_name.ToLower()) < 0)
                //        {
                //            player.skill_name = obj.skillsList[Int32.Parse(player.skill_name) - 1];
                //            player.skill_id = obj.skillsList.FindIndex(a => a.ToLower() == player.skill_name.ToLower()) + 1;
                //        }
                //    }
                //}

                return vPLayer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion      

    }
}
