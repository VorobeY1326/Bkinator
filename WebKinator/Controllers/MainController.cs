using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Web.Mvc;
using Bkinator;
using WebKinator.Models;
using WebKinator.State;
using System.Linq;

namespace WebKinator.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        private string StateFilename
        {
            get { return Path.Combine(Server.MapPath("/Content/"), "save.bin"); }
        }

        private BkinatorState state;

        private BkinatorState State
        {
            get { return state ?? (state = StateSaver.GetState(StateFilename)); }
        }

        public ActionResult Index()
        {
            PlayerState.ResetPlayerState(Session);
            return View();
        }

        [HttpGet]
        public ActionResult Play()
        {
            var playerState = PlayerState.GetPlayerState(Session, State.Genie);
            if (playerState.CurrentQuestionId == null)
            {
                PlayerState.ResetPlayerState(Session);
                return RedirectToAction("Play");
            }
            return View(new PLayModel
                {
                    QuestionString = State.Questions[playerState.CurrentQuestionId].Text,
                    Guesses = playerState.AnswerGuesses.Select(g => Tuple.Create(State.Answers[g.AnswerId].Text, g.Probability)).ToList()
                });
        }

        [HttpPost]
        public ActionResult Play(int? choiceId)
        {
            if (choiceId == null)
                return new EmptyResult();
            var playerState = PlayerState.GetPlayerState(Session, State.Genie);
            if (playerState.AnsweredQuestions.All(a => a.QuestionId != playerState.CurrentQuestionId))
                playerState.AnsweredQuestions.Add(new AnsweredQuestion
                    {
                        QuestionId = playerState.CurrentQuestionId, Choise = choiceId.Value
                    });
            playerState.AnswerGuesses = State.Genie.GetAnswerGuesses(playerState.AnsweredQuestions);
            playerState.CurrentQuestionId = State.Genie.GetNextQuestionId(playerState.AnsweredQuestions, playerState.AnswerGuesses);
            if (playerState.CurrentQuestionId == null)
            {
                if (playerState.AnswerGuesses.Count == 0)
                    return new EmptyResult();
                return RedirectToAction("Answer", new {answerId = playerState.AnswerGuesses[0].AnswerId});
            }
            return Play();
        }

        public ActionResult Answer(string answerId)
        {
            if (answerId == null || !State.Answers.ContainsKey(answerId))
                return new EmptyResult();
            return View(Tuple.Create(answerId, State.Answers[answerId]));
        }

        public ActionResult TryAnotherAnswer()
        {
            var playerState = PlayerState.GetPlayerState(Session, State.Genie);
            if (playerState.AnswerGuesses.Count == 0 || playerState.AnsweredQuestions.Count == 0 || playerState.CurrentQuestionId != null)
                return new EmptyResult();
            return View(playerState.AnswerGuesses.Select(g => Tuple.Create(State.Answers[g.AnswerId].Text, g)).ToList());
        }

        public ActionResult AddAnswer()
        {
            var playerState = PlayerState.GetPlayerState(Session, State.Genie);
            if (playerState.AnsweredQuestions.Count == 0)
                return new EmptyResult(); //TODO error
            return View();
        }

        [HttpPost]
        public ActionResult AddAnswer(string answer, string description)
        {
            var answeredQuestions = PlayerState.GetPlayerState(Session, State.Genie).AnsweredQuestions;
            if (string.IsNullOrWhiteSpace(answer) || string.IsNullOrWhiteSpace(description) || answeredQuestions.Count == 0)
                return new EmptyResult(); //TODO error
            var answerId = State.AddNewAnswer(new Answer{ Text = answer, Description = description }, answeredQuestions);
            StateSaver.SaveState(State, StateFilename);
            return RedirectToAction("AddQuestion", new { answerId }); //TODO some happy message before?
        }

        public ActionResult AnswerCorrect(string answerId)
        {
            if (answerId == null || !State.Answers.ContainsKey(answerId))
                return new EmptyResult();
            var playerState = PlayerState.GetPlayerState(Session, State.Genie);
            if (playerState.AnsweredQuestions.Count > 0 && playerState.AnswerGuesses.Count > 0)
                State.Genie.UpdateStatisticsByAnswerGuessed(answerId, playerState.AnsweredQuestions);
            StateSaver.SaveState(State, StateFilename);
            PlayerState.ResetPlayerState(Session);
            return View();
        }

        [HttpGet]
        public ActionResult AddQuestion(string answerId)
        {
            var playerState = PlayerState.GetPlayerState(Session, State.Genie);
            if (string.IsNullOrWhiteSpace(answerId) || playerState.AnsweredQuestions.Count == 0 || playerState.AnswerGuesses.Count == 0)
                return new EmptyResult();
            var answerIds = new List<string> {answerId};
            answerIds.AddRange(playerState.AnswerGuesses.Select(g => g.AnswerId).Take(10));
            return View(new AddQuestionModel
                {
                    AnswerIdsAndNames = answerIds.Select(id => Tuple.Create(id, State.Answers[id].Text)).ToList()
                }); //TODO is always answerids are correct in playerstate?
        }

        [HttpPost]
        public ActionResult AddQuestion(AddQuestionModel model)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.Question)
                && model.Answers != null && model.Answers.Any(a => a.Value != AddQuestionModel.DontknowName))
            {
                var knownAnswers = new List<Tuple<string, int>>();
                foreach (var answer in model.Answers.Where(a => a.Value != AddQuestionModel.DontknowName))
                {
                    var id = answer.Key;
                    if (!State.Answers.ContainsKey(id))
                        continue;
                    var choiceString = Choices.Names.Select((n,i) => Tuple.Create(n,i)).FirstOrDefault(t => t.Item1 == answer.Value);
                    if (choiceString == null)
                        continue;
                    int choice = choiceString.Item2;
                    knownAnswers.Add(Tuple.Create(id, choice));
                }
                if (knownAnswers.Count > 0)
                    State.AddNewQuestion(new Question {Text = model.Question}, knownAnswers);
                StateSaver.SaveState(State, StateFilename);
            }
            return RedirectToAction("Index");
        }
    }
}