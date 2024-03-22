using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bodog.Predictor.Admin.Models;
using Bodog.Predictor.Interfaces.Admin;
using Bodog.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Contracts.Notification;
using System.Data;
using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Library.Utility;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Contracts.Enums;

namespace Bodog.Predictor.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly Blanket.DataPopulation.Populate _PopulateDBContext;
        private readonly Blanket.Feeds.Ingestion _IngestionDBContext;
        private readonly Blanket.Management.Tour _TourContext;
        private readonly Blanket.Management.Series _SeriesContext;
        private readonly Blanket.Simulation.Simulation _SimulationContext;
        private readonly Blanket.BackgroundServices.GameLocking _GameLocking;
        //private readonly Blanket.Feeds.Ingestion _Ingestion;
        private readonly Blanket.Notification.Publish _NotificationPublishContext;
        private readonly Blanket.Scoring.PlayerStatistics _PlayerStatistics;
        private readonly Blanket.Leaderboard.Leaderbaord _LeaderBoard;
        private readonly Blanket.Analytics.Analytics _AnalyticsDBContext;
        private readonly Blanket.Feeds.Gameplay _GameplayBlanketContext;
        private readonly Blanket.AdminQuestions.AdminQuestions _QuestionsContext;
        private readonly Blanket.Scoring.Process _ScoringContext;
        private readonly Int32 _OptionsCount;

        public HomeController(IOptions<Application> appSettings, ISession session, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, session, aws, postgre, redis, cookies, asset)
        {
            _PopulateDBContext = new Blanket.DataPopulation.Populate(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _IngestionDBContext = new Blanket.Feeds.Ingestion(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _TourContext = new Blanket.Management.Tour(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _SeriesContext = new Blanket.Management.Series(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _SimulationContext = new Blanket.Simulation.Simulation(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _GameLocking = new Blanket.BackgroundServices.GameLocking(_AppSettings, null, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            // _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublishContext = new Blanket.Notification.Publish(appSettings, aws, postgre, redis, cookies, asset);
            //_TemplateContext = new Blanket.Template.Markup(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _PlayerStatistics = new Blanket.Scoring.PlayerStatistics(appSettings, aws, postgre, redis, cookies, asset);
            _LeaderBoard = new Blanket.Leaderboard.Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
            _AnalyticsDBContext = new Blanket.Analytics.Analytics(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _GameplayBlanketContext = new Blanket.Feeds.Gameplay(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _QuestionsContext = new Blanket.AdminQuestions.AdminQuestions(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _ScoringContext = new Blanket.Scoring.Process(appSettings, aws, postgre, redis, cookies, asset);
            _OptionsCount = appSettings.Value.Properties.OptionCount;
        }

        #region " LOGIN "

        [HttpGet]
        //[Route("/admin")]
        //[Route("/admin/login")]
        public IActionResult Login(string enc)
        {
            #region " Decryption "

            ViewBag.Enc = Library.Utility.GenericFunctions.DecryptedValue(enc);

            #endregion

            if (_Session._HasAdminCookie)
                Response.Redirect("/admin/" + _Session.Pages().FirstOrDefault().Replace(" ", ""));

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (Authorization authority in _Admin.Authorization)
                {
                    if (model.Username.ToLower().Trim() == authority.User.ToLower().Trim() && model.Password == authority.Password)
                    {
                        bool status = _Session.SetAdminCookie(model.Username);

                        if (status)
                            Response.Redirect("/admin/" + _Session.Pages(model.Username).FirstOrDefault().Replace(" ", ""));
                        else
                        {
                            ViewBag.MessageType = "_Error";
                            ViewBag.MessageText = "Session is Invalid.";
                        }
                    }
                    else
                    {
                        ViewBag.MessageType = "_Error";
                        ViewBag.MessageText = "Incorrect login credentials.";
                    }
                }
            }

            return View();
        }

        #endregion

        #region " DATA POPULATION "

        [HttpGet]
        public IActionResult DataPopulation(Int32 tournament, Int32 series)
        {
            DataPopulationModel model = new DataPopulationWorker().GetModel(_TourContext, _SeriesContext, tournament, series);
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DataPopulation(DataPopulationModel model, String process)
        {
            Int32 tournament = model.TournamentId;
            Int32 series = model.SeriesId;
            String league = model.League ?? "";
            Int32 retVal = -40;

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "tournament":
                        retVal = _PopulateDBContext.SaveTournament();
                        break;
                    case "series":
                        retVal = _PopulateDBContext.SaveSeries();
                        break;
                    case "fixtures":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.SaveFixtures(tournament, series, league);
                        else
                            retVal = -30;
                        break;
                    case "players":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.SavePlayers(tournament, series, league);
                        else
                            retVal = -30;
                        break;
                    case "teams":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.SaveTeams(tournament, series, league);
                        else
                            retVal = -30;
                        break;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please select both tournament and series.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} populated successfully."
                            : $"Error while populating {process}. RetVal = {retVal}";
                    }
                }
            }

            //Doesn't need to pass tournament and series as it it already initialized in model object.
            DataPopulationModel dataModel = new DataPopulationWorker().GetModel(_TourContext, _SeriesContext, tournament, series);
            return View(dataModel);
        }

        #endregion

        #region " FEED INGESTION "

        [HttpGet]
        public IActionResult FeedIngestion()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> FeedIngestion(FeedIngestionModel model, String process)
        {
            Int32 retVal = -40;

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "languages":
                        retVal = await _IngestionDBContext.Languages();
                        break;
                    case "fixtures":
                        {
                            retVal = await _IngestionDBContext.Fixtures();
                            if (retVal == 1)
                                retVal = await _IngestionDBContext.IngestNotificationStatus();
                        }
                        break;
                    case "skills":
                        retVal = await _IngestionDBContext.Skills();
                        break;
                    case "questions":
                        if (model.QuestionsMatchID == null)
                            break;
                        retVal = await _IngestionDBContext.Questions(Convert.ToInt32(model.QuestionsMatchID));
                        break;
                    case "allmatchquestions":
                        retVal = await _IngestionDBContext.AllMatchQuestions();
                        break;
                    case "teamsrecentresults":
                        retVal = await _IngestionDBContext.GetRecentResults();
                        break;
                    case "matchstatus":
                        if (model.MatchId == null)
                            break;
                        retVal = await _IngestionDBContext.MatchInningStatus(Convert.ToInt32(model.MatchId));
                        break;
                    case "leaderboard":
                        if (model.LeaderBoardGamedayId == null && model.LeaderBoardPhaseId == null)
                            break;
                        retVal = await _IngestionDBContext.LeaderBoard(Convert.ToInt32(model.LeaderBoardGamedayId), Convert.ToInt32(model.LeaderBoardPhaseId));
                        break;
                    case "currentgamedaymatches":
                        retVal = await _IngestionDBContext.CurrentGamedayMatches();
                        break;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
                else
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide the input values.";
                }
            }

            return View();
        }

        #endregion

        #region " SIMULATION "

        [HttpGet]
        public IActionResult Simulation()
        {
            SimulationModel model = new SimulationWorker().GetModel(_SimulationContext, null);
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Simulation(SimulationModel model, String process)
        {
            Int32 retVal = -40;

            if (ModelState.IsValid)
            {
                try
                {
                    switch (process)
                    {
                        case "matchprocess":
                            retVal = _SimulationContext.SubmitMatchForProcess(Convert.ToInt32(model.MatchId));
                            break;
                        case "generateuser":
                            retVal = _SimulationContext.GenerarateUser(Convert.ToInt32(model.UserCount));
                            break;
                        case "generateuserpredictions":
                            retVal = _SimulationContext.GenerarateUserPredictioons(Convert.ToInt32(model.GenenrateUserPredictionMatchId), Convert.ToInt32(model.OptionId));
                            break;
                        //case "userpointprocess":
                        //    if (model.GamedayId == null)
                        //        break;
                        //    retVal = _SimulationContext.GenerarateUserPredictioons(Convert.ToInt32(model.GamedayId), Convert.ToInt32(model.matchdayId));
                        //    break;
                        case "masterdatarollback":
                            retVal = _SimulationContext.MasterdataRollback();
                            break;
                        case "userdatarollback":
                            retVal = _SimulationContext.UserdataRollback();
                            break;
                        case "matchlocking":
                            retVal = _GameLocking.Lock(1, Convert.ToInt32(model.LockMatchId), 0);
                            await IngestThis(Convert.ToInt32(model.LockMatchId));
                            break;
                        case "inninglocking":
                            if (model.InningId == null)
                                break;
                            retVal = _GameLocking.Lock(1, Convert.ToInt32(model.LockInningMatchId), Convert.ToInt32(model.InningId));
                            await IngestThis(Convert.ToInt32(model.LockInningMatchId));
                            break;
                        case "inningunlocking":
                            if (model.UnlockInningId != null)
                            {
                                retVal = _GameLocking.UnLock(1, Convert.ToInt32(model.UnlockInningMatchId), Convert.ToInt32(model.UnlockInningId));
                                await IngestThis(Convert.ToInt32(model.UnlockInningMatchId));
                            }
                            break;
                        case "submitMatchLineups":
                            retVal = _SimulationContext.SubmitMatchLineups(model.MatchFile);
                            break;
                        case "submitMatchToss":
                            retVal = _SimulationContext.SubmitMatchToss(model.TossMatchFile);
                            break;
                        case "matchanswers":
                            retVal = _SimulationContext.SubmitMatchAnswers(Convert.ToInt32(model.SubmitAnswerMatchId));
                            break;
                        case "pointcalculation":
                            retVal = _SimulationContext.RunPointCalculation();
                            break;
                        case "updatematchdatetime":
                            retVal = _SimulationContext.UpdateMatchDateTime(Convert.ToInt32(model.UpdateMatchId), model.UpdateMatchDateTime);
                            break;
                        case "abandonmatch":
                            if (model.AbandonMatchId == null)
                                break;
                            retVal = _QuestionsContext.AbandonMatch(Convert.ToInt32(model.AbandonMatchId));
                            await IngestThis(Convert.ToInt32(model.AbandonMatchId));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Contracts.Common.HTTPLog httpLog = _Cookies.PopulateLog(" ADMIN SIMULATION ERROR: ", ex.Message);
                    _AWS.AppendS3Logs(httpLog);
                }

                ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                ViewBag.MessageText = (retVal == 1) ? $"{process} submited successfully."
                    : $"Error while ingesting {process}. RetVal = {retVal}";
            }
            model = new SimulationWorker().GetModel(_SimulationContext, null);
            return View(model);
        }

        #endregion

        #region " Notification "

        [HttpGet]
        public IActionResult Notification()
        {
            NotificationModel model = new NotificationWorker().GetModel();
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Notification(NotificationModel model, String process)
        {
            Int32 retVal = -40;
            Boolean success = false;

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "ingesttopics":
                        retVal = await _IngestionDBContext.InsertTopics();
                        break;
                    case "sendnotification":
                        if (model.NotificationPlatformId == "0" || String.IsNullOrEmpty(model.NotificationText))
                            break;
                        List<NotificationPlatforms> listPlatform = new List<NotificationPlatforms>();
                        if (model.NotificationPlatformId == "1")
                            listPlatform.Add(NotificationPlatforms.Android);
                        else if (model.NotificationPlatformId == "2")
                            listPlatform.Add(NotificationPlatforms.IOS);
                        else if (model.NotificationPlatformId == "3")
                        {
                            listPlatform.Add(NotificationPlatforms.Android);
                            listPlatform.Add(NotificationPlatforms.IOS);
                        }
                        foreach (NotificationPlatforms p in listPlatform)
                        {
                            List<NotificationMessages> notificationList = new List<NotificationMessages>();
                            NotificationMessages n = new NotificationMessages();

                            n.EventId = (Int32)Contracts.Notification.NotificationEvents.Generic;
                            n.Language = "en";
                            n.Message = model.NotificationText;
                            n.Subject = "Generic";
                            notificationList.Add(n);
                            success = await _NotificationPublishContext.SendPushNotification(p, notificationList, false, model.NotificationMatch, model.NotificationMatch == null ? "overall" : "");
                            if (success)
                                retVal = 1;
                        }
                        break;
                    case "ingestnotificationmessages":
                        if (!String.IsNullOrEmpty(model.NotifcationTextJson))
                            retVal = await _IngestionDBContext.IngestNotificationMessages(model.NotifcationTextJson);
                        break; ;
                    case "notificationStatus":
                        retVal = await _IngestionDBContext.IngestNotificationStatus();
                        break;
                }

                ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                ViewBag.MessageText = (retVal == 1) ? $"{process} submited successfully."
                    : $"Error while ingesting {process}. RetVal = {retVal}";
            }
            model = new NotificationWorker().GetModel();
            return View(model);
        }

        #endregion

        #region " MatchAnswers "
        [HttpGet]
        public IActionResult MatchAnswers(Int32 matchId, String matchFile)
        {
            //NotificationModel model = new NotificationWorker().GetModel();
            //  MatchAnswersModel model = new MatchAnswersWorker().GetModel(_GameLocking, _PlayerStatistics, matchId, matchFile);
            MatchAnswersModel model = new MatchAnswersModel();
            return View(model);
        }

        #endregion

        #region " LOG ME OUT "

        [HttpGet]
        public IActionResult Logout()
        {
            _Session.DeleteAdminCookie();
            Response.Redirect("/admin/" + "Login");
            return Content("");
        }

        #endregion

        #region " LeaderBoard "
        [HttpGet]
        public IActionResult AdminLeaderBoard()
        {
            LeaderBoardModel model = new LeaderBoardWorker().GetModel(_LeaderBoard);
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AdminLeaderBoard(LeaderBoardModel model, String process)
        {
            Int32 retVal = -40;
            Reports Leaderboard = new Reports();
            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "getusertoprank":

                        if (model.GamedayId == null && model.PhaseId == null && model.TopUser == null)
                            retVal = -40;
                        else
                            retVal = _LeaderBoard.AdminLeaderBoard(Convert.ToInt32(model.TopUser), Convert.ToInt32(model.LeaderBoardTypeId), Convert.ToInt32(model.GamedayId), Convert.ToInt32(model.PhaseId), out Leaderboard);

                        break;
                }
                model = new LeaderBoardWorker().GetModel(_LeaderBoard);
                model.LeaderBoardList = Leaderboard.LeaderBoardList;

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
                else
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide the input values.";
                }
            }

            return View(model);
        }

        #endregion " LeaderBoard "

        #region " ANALYTICS "
        [HttpGet]
        public IActionResult Analytics()
        {
            String mError = String.Empty;
            String mAnalytics = _AnalyticsDBContext.GetAnalytics(ref mError);//.GetModel(_TourContext, _SeriesContext, tournament, series);

            ViewBag.HTML = mAnalytics;
            ViewBag.Error = mError;
            return View();
        }
        #endregion " ANALYTICS "

        #region " QUESTIONS "

        [HttpGet]
        public IActionResult MatchSchedule(Int32 retVal = 1)
        {
            if (retVal != 1)
            {
                ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                ViewBag.MessageText = (retVal == 1) ? $"Question saved successfully."
                    : $"Something went wrong, please select date again.";
            }

            Schedule schedule = new Schedule();
            schedule.Fixtures = new List<Contracts.Feeds.Fixtures>();
            schedule.MatchDate = DateTime.Now;
            return View(schedule);
        }

        [HttpPost]
        public IActionResult MatchSchedule(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                schedule.ShortMatchDate = schedule.MatchDate.ToString("MM/dd/yyyy");
                HTTPResponse response = _GameplayBlanketContext.GetFixtures("en").Result;
                List<Fixtures> fixtures = fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)response.Data).Value)); //GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(response.Data)).Value.ToString());
                schedule.Fixtures = fixtures.Where(a => GenericFunctions.ToUSCulture(a.Date).Date == schedule.MatchDate.Date).ToList();

                foreach (Fixtures fixture in schedule.Fixtures)
                {
                    fixture.Date = (fixture.Date.USFormatDate()).ToString("dd/MM/yyyy hh:mm:ss tt");
                }

                return View(schedule);
            }
            else
            {
                schedule.Fixtures = new List<Contracts.Feeds.Fixtures>();
                return View(schedule);
            }
        }

        [HttpGet]
        public IActionResult Questions(Int32 matchId, String header, String questionStatus = "-2", Int32 retVal = 0, String messageText = null)
        {
            QuestionsModel model = new QuestionsModel(); //QuestionsWorker().GetModel();

            if (matchId == 0)
            {
                return RedirectToAction("MatchSchedule", "Home", new { retVal = -1 });
            }

            if (retVal != 0)
            {
                ViewBag.MessageType = retVal == 1 ? "_Success" : "_Error";
                ViewBag.MessageText = messageText;
            }

            model = new QuestionsWorker().GetModel(_SimulationContext);
            model.matchQuestions = new List<MatchQuestions>();
            model.MatchId = matchId;
            model.QuestionStatus = questionStatus;
            model.Header = header;

            model.matchQuestions = _QuestionsContext.GetFilteredQuestions(matchId, questionStatus);

            foreach (MatchQuestions questions in model.matchQuestions)
            {
                for (int i = 0; i < _OptionsCount; i++)
                {
                    if (i >= questions.Options.Count)
                    {
                        questions.Options.Add(new Contracts.Feeds.Options
                        {
                            QuestionId = questions.QuestionId,
                            OptionId = i,
                            OptionDesc = String.Empty
                        });
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Questions(QuestionsModel model)
        {
            String questionStatus = model.QuestionStatus;
            Int32 matchId = model.MatchId;

            return RedirectToAction("Questions", "Home", new { matchId = model.MatchId, questionStatus = model.QuestionStatus });
        }

        [HttpGet]
        public IActionResult AddEditQuestions(Int32 matchId, Int32 questionId, String questionType, String header)
        {
            MatchQuestions model = new MatchQuestions();
            model.MatchId = matchId;
            model.QuestionId = questionId;
            model.QuestionType = questionType;
            if (questionId == 0)
            {
                model.Options = new List<Contracts.Feeds.Options>();
            }
            else
            {
                model = _QuestionsContext.GetMatchQuestionsDetail(matchId, questionId);
            }

            for (int i = model.Options.Count; i < _OptionsCount; i++)
            {
                Contracts.Feeds.Options option = new Contracts.Feeds.Options();
                model.Options.Add(option);
            }

            ViewBag.Header = header;
            return View(model);
        }

        [HttpPost]
        public IActionResult AddEditQuestions(MatchQuestions model)
        {
            Int32 retVal = -30;
            Int32 matchId = model.MatchId;
            Int32 questionId = model.QuestionId;
            String questionType = model.QuestionType;
            HTTPResponse response = _GameplayBlanketContext.GetFixtures("en").Result;
            List<Fixtures> fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)response.Data).Value));
            Fixtures fixture = fixtures.Where(a => a.MatchId == model.MatchId).FirstOrDefault();
            String header = fixture.TeamAName + " VS " + fixture.TeamBName;// TempData["Header"].ToString();
            String error = String.Empty;

            model.Options = model.Options.Where(a => a.OptionDesc != String.Empty && a.OptionDesc != null && a.OptionDesc != "").ToList();

            if (model.Options.Count <= 1)
            {
                return RedirectToAction("AddEditQuestions", "Home", new { matchId = model.MatchId, questionId = 0, questionType = model.QuestionType, header = header });
            }

            model.QuestionStatus = model.Options.Any(a => a.IsCorrectBool && model.QuestionType == "QS_PRED") ? Convert.ToInt32(QuestionStatus.Resolved) : model.QuestionStatus;

            retVal = _QuestionsContext.SaveQuestions(model);

            Int32 questionstatus;
            String successText;
            if (model.QuestionStatus != Convert.ToInt32(QuestionStatus.Unpublished))
            {
                questionstatus = Convert.ToInt32(QuestionStatus.Published);
                successText = $"Question {((QuestionStatus)model.QuestionStatus).ToString()} successfully.";
            }
            else
            {
                questionstatus = model.QuestionStatus;
                successText = $"Question saved successfully.";
            }

            if (retVal == 1)
                retVal = _IngestionDBContext.Questions(model.MatchId).Result;
            else
                error = "Error while saving questions. ";

            if (retVal == 1)
                return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionstatus.ToString(), retVal = retVal, messageText = successText });
            else
                error = "Error while Ingesting questions. ";

            ViewBag.Header = header;
            ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
            ViewBag.MessageText = (retVal == 1) ? $"Question saved successfully."
                : error + $"RetVal = {retVal}";

            if (model.Options.Count < _OptionsCount)
            {
                for (int i = model.Options.Count; i < _OptionsCount; i++)
                {
                    Contracts.Feeds.Options option = new Contracts.Feeds.Options();
                    model.Options.Add(option);
                }
            }

            return View(model);
        }

        public IActionResult EditingQuestionStatus(Int32 matchId, Int32 questionId, Int32 questionStatus, String header)
        {
            Int32 retVal = -30;
            MatchQuestions matchQuestionModel = _QuestionsContext.GetMatchQuestionsDetail(matchId, questionId);
            matchQuestionModel.QuestionStatus = questionStatus;
            retVal = _QuestionsContext.SaveQuestions(matchQuestionModel);

            if (retVal == 1)
                retVal = _IngestionDBContext.Questions(matchId).Result;

            String questionStatusString = ((QuestionStatus)questionStatus).ToString();
            String messageText = retVal == 1 ? $"Question {questionStatusString} successfully." : $"Error while {questionStatusString} question.";

            //if (retVal == 1)
            //    return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText });

            return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText }); ;
        }

        public IActionResult SendNotification(Int32 matchId, String notificationText, Int32 questionStatus, String header)
        {
            Int32 retVal = -30;
            List<NotificationPlatforms> listPlatform = new List<NotificationPlatforms>();

            listPlatform.Add(NotificationPlatforms.Android);
            listPlatform.Add(NotificationPlatforms.IOS);

            foreach (NotificationPlatforms p in listPlatform)
            {
                List<NotificationMessages> notificationList = new List<NotificationMessages>();
                NotificationMessages n = new NotificationMessages();
                Boolean success = false;

                n.EventId = (Int32)Contracts.Notification.NotificationEvents.Generic;
                n.Language = "en";
                n.Message = notificationText;
                n.Subject = "Generic";
                notificationList.Add(n);
                success = _NotificationPublishContext.SendPushNotification(p, notificationList, false, matchId).Result;
                if (success)
                    retVal = 1;
            }

            String questionStatusString = ((QuestionStatus)questionStatus).ToString();
            String messageText = retVal == 1 ? $"Notification sent successfully." : $"Error while sending notification";

            //if (retVal == 1)
            //    return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText });

            return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText });
        }

        public IActionResult PointCalculation(Int32 matchId, Int32 questionStatus, String header)
        {
            Int32 retVal = -30;
            String lang = "en";
            bool success = false;

            List<Fixtures> fixtures = new List<Fixtures>();
            Fixtures mMatchFixture = new Fixtures();
            HTTPResponse httpResponse = _GameplayBlanketContext.GetFixtures(lang).Result;


            if (httpResponse.Meta.RetVal == 1)
            {
                if (httpResponse.Data != null)
                    fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                mMatchFixture = fixtures.Where(c => c.MatchId == matchId).FirstOrDefault();
            }
            else
                throw new Exception("GetFixtures RetVal is not 1.");

            MatchFeed mMatchFeed = _GameLocking.GetMatchScoresFeed(mMatchFixture.Matchfile.ToString());
            if (mMatchFeed.Matchdetail.Status.ToLower().Trim() == "match ended")
            {
                if (mMatchFixture != null && !String.IsNullOrEmpty(mMatchFixture.Matchfile))
                {
                    success = _ScoringContext.CalculateAnswers(mMatchFixture);

                    if (success)
                        success = _ScoringContext.QuestionAnswerProcessUpdate(matchId);
                    if (success)
                        success = _ScoringContext.SubmitMatchWinTeam(mMatchFixture);
                    if (success)
                        success = LockInning(matchId, -1);

                    ViewBag.MessageType = success ? "_Success" : "_Error";
                    ViewBag.MessageText = success ? $"Points submitted successfully. MatchId = {matchId}"
                        : $"Error while submiting match points. MatchId = {matchId}";

                }
            }
            else
            {
                ViewBag.MessageType = "_Error";
                ViewBag.MessageText = "Match has not been ended yet.";
            }
            return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal = ViewBag.MessageType == "_Error" ? -1 : 1, messageText = ViewBag.MessageText });
        }

        public bool LockInning(Int32 matchId, Int32 inningNo)
        {
            Int32 lockVal = 0, optType = 1;
            try
            {
                lockVal = _GameLocking.Lock(optType, matchId, inningNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (lockVal == 1);
        }

        public IActionResult AbandonMatch(Int32 matchId, Int32 abandonedMatchId, Int32 questionStatus, String header)
        {
            Int32 retVal = -30;
            String error = String.Empty;

            retVal = _QuestionsContext.AbandonMatch(abandonedMatchId);

            if (retVal == 1)
                retVal = IngestThis(abandonedMatchId).Result;
            else
            {
                error = $"There was problem in abandoning MatchId = {abandonedMatchId}";
                return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal = retVal, messageText = error });
            }

            String messageText = retVal == 1 ? $"MatchId = {abandonedMatchId} abandoned successfully." :
                                                $"Error while ingesting Fixtures.";

            return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal = retVal, messageText = messageText });
        }

        #endregion " QUESTIONS "


        #region " UserDetailsReport "

        [HttpGet]
        public IActionResult UserDetailsReport()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDetailsReport(String process)
        {
            Int32 retVal = -40;
            String error = "";
            String _HTML = "";

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "generateuserdetailsreport":
                        _HTML = _AnalyticsDBContext.GetUserDetailsReport(ref error);

                        if(!String.IsNullOrEmpty(_HTML))
                        {
                           await _Asset.SET(_Asset.UserDetailsReport(), _HTML, false);
                        }
                        break;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
                else
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide the input values.";
                }
            }

            return View();
        }

        #endregion

        public async Task<int> IngestThis(Int32 matchId)
        {
            Int32 retval = await _IngestionDBContext.Fixtures();
            retval = await _IngestionDBContext.MatchInningStatus(matchId);
            retval = await _IngestionDBContext.Questions(matchId);
            retval = await _IngestionDBContext.CurrentGamedayMatches();

            return retval;
        }
        /*--------------------------------*/
    }
}
