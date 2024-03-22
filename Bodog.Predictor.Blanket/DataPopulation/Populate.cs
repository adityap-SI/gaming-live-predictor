using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Blanket.DataPopulation
{
    public class Populate : Common.BaseBlanket
    {
        private readonly DataAccess.DataPopulation.Populate _DBContext;
        private readonly Int32 _TourId;
        private Feed _Feed;

        public Populate(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.DataPopulation.Populate(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }


        public Int32 SaveTournament(String league = "")
        {
            _Feed = new Feed(_AppSettings, league);

            IList<SITournament> vTournament = _Feed.GetTournaments();

            int[] arr_tour_id = vTournament.Select(c => c.tour_id).ToArray();
            string[] arr_tour_name = vTournament.Select(c => c.tour_name).ToArray();
            string[] arr_tour_display_name = vTournament.Select(c => c.tour_name).ToArray();
            DateTime[] arr_tour_start_date = vTournament.Select(c => c.tour_start_date).ToArray();
            DateTime[] arr_tour_end_date = vTournament.Select(c => c.tour_end_date).ToArray();

            return _DBContext.SaveTournament(arr_tour_id, arr_tour_name, arr_tour_display_name, arr_tour_start_date, arr_tour_end_date);
        }

        public Int32 SaveSeries(String league = "")
        {
            _Feed = new Feed(_AppSettings, league);

            IList<SISeries> vSeries = _Feed.GetSeries();
            int[] arr_tour_id = vSeries.Select(c => c.tour_id).ToArray();
            int[] arr_series_id = vSeries.Select(c => c.series_Id).ToArray();
            string[] arr_series_name = vSeries.Select(c => c.seriesname).ToArray();
            string[] arr_series_display_name = vSeries.Select(c => c.seriesname).ToArray();
            DateTime[] arr_series_start_date = vSeries.Select(c => c.series_start_date).ToArray();
            DateTime[] arr_series_end_date = vSeries.Select(c => c.series_end_date).ToArray();

            string[] arr_comp_type = vSeries.Select(c => c.comp_type).ToArray();

            List<int> arr_comp_type_id = new List<int>();
            foreach (var item in arr_comp_type)
            {
                int key = new SeriesFormat(item.ToLower()).Value + 1;
                arr_comp_type_id.Add(key);
            }

            return _DBContext.SaveSeries(arr_tour_id, arr_series_id, arr_series_name, arr_series_display_name, arr_series_start_date, arr_series_end_date, arr_comp_type_id);
        }

        public async Task<Int32> SaveFixtures(int tournamentId, int seriesId, String league = "")
        {
            try
            {
                foreach (String lang in await GetLanguages())
                {
                    _Feed = new Feed(_AppSettings, league, lang);

                    IList<SIMatch> vSeries = _Feed.GetMatches(tournamentId.ToString(), seriesId.ToString());
                    int[] array_matchid = vSeries.Select(c => c.match_Id).ToArray();
                    int[] array_home_teamid = vSeries.Select(c => c.home_team_id).ToArray();
                    string[] array_series_home_team_name = vSeries.Select(c => c.home_team_name).ToArray();
                    string[] array_series_home_team_short = vSeries.Select(c => c.home_team_short_name).ToArray();

                    int[] array_away_teamid = vSeries.Select(c => c.away_team_id).ToArray();
                    string[] array_series_away_team_name = vSeries.Select(c => c.away_team_name).ToArray();
                    string[] array_series_away_team_short = vSeries.Select(c => c.away_team_short_name).ToArray();

                    DateTime[] array_match_date = vSeries.Select(c => c.matchdate_ist).ToArray();
                    string[] array_match_name = vSeries.Select(c => c.match_name).ToArray();

                    string[] array_matchType = vSeries.Select(c => c.match_type).ToArray();
                    string[] array_matchtime_local = vSeries.Select(c => c.match_time_local).ToArray();
                    string[] array_matchtime_ist = vSeries.Select(c => c.match_time_ist).ToArray();
                    string[] array_matchtime_gmt = vSeries.Select(c => c.match_time_gmt).ToArray();
                    string[] array_matchStatus = vSeries.Select(c => c.match_status).ToArray();
                    string[] array_matchResult = vSeries.Select(c => c.match_result).ToArray();
                    string[] array_matchFile = vSeries.Select(c => c.match_file).ToArray();
                    string[] array_matchNum = vSeries.Select(c => c.match_number).ToArray();
                    string[] array_venue = vSeries.Select(c => c.venue).ToArray();

                    _DBContext.SaveFixtures(tournamentId, seriesId, array_matchid, array_home_teamid, array_series_home_team_name, array_series_home_team_short,
                        array_away_teamid, array_series_away_team_name, array_series_away_team_short, array_match_date, array_match_name, array_matchType, array_matchtime_local,
                        array_matchtime_ist, array_matchtime_gmt, array_matchStatus, array_matchResult, array_matchFile, array_matchNum, array_venue);
                }

                return 1;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }

        public async Task<Int32> SaveTeams(int tournamentId, int seriesId, String league = "")
        {
            foreach (String lang in await GetLanguages())
            {
                _Feed = new Feed(_AppSettings, league, lang);

                IList<SITeam> vTeams = _Feed.GetTeams(tournamentId.ToString(), seriesId.ToString());

                int[] array_teamid = vTeams.Select(c => c.team_id).ToArray();
                string[] array_team_name = vTeams.Select(c => c.team_name).ToArray();
                string[] array_team_short = vTeams.Select(c => c.team_short).ToArray();
                _DBContext.SaveTeams(tournamentId, seriesId, array_teamid, array_team_name, array_team_short, lang);

            }
            return 1;
        }

        public async Task<Int32> SavePlayers(int tournamentId, int seriesId, String league = "")
        {
            Int32 retVal = -50;

            foreach (String lang in await GetLanguages())
            {
                _Feed = new Feed(_AppSettings, league, lang);

                IList<SITeam> mAllTeams = _Feed.GetTeams(tournamentId.ToString(), seriesId.ToString());

                IList<Skills> skills = await GetSkills(lang);

                foreach (SITeam mTeam in mAllTeams)
                {
                    IList<SIPlayer> vPlayers = _Feed.GetPlayers(mTeam.team_id.ToString(), tournamentId.ToString(), seriesId.ToString(), lang, out int optType);

                    if (vPlayers == null)
                        break;

                    int team_id = vPlayers.Select(c => c.team_id).FirstOrDefault();
                    int[] array_playerid = vPlayers.Select(c => c.playerid).ToArray();
                    string[] array_player_name = vPlayers.Select(c => c.player_name).ToArray();
                    string[] array_display_name = vPlayers.Select(c => c.player_display_name).ToArray();
                    string[] array_display_skill = vPlayers.Select(c => c.skill_name).ToArray();

                    List<int> arr_skill_id = new List<int>();

                    try
                    {
                        foreach (var item in array_display_skill)
                        {
                            int key = 1;

                            if (item != "")
                            {
                                foreach (var skill_name in skills)
                                {
                                    Int32 skillComapre = String.Compare(skill_name.SkillName, item.ToString(), true);
                                    if (skillComapre == 0)
                                    {
                                        key = skill_name.SkillId;
                                        break;
                                    }
                                }
                            }
                            //key = new PlayerSkills(item.ToLower()).Value;

                            arr_skill_id.Add(key);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    retVal = _DBContext.SavePlayersPerTeam(optType, lang, tournamentId, seriesId, team_id, array_playerid, array_player_name, array_display_name,
                        array_display_skill, arr_skill_id);

                    if (retVal != 1) break;
                }
            }
            return retVal;
        }

    }
}