using System;
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

        private static readonly BkinatorState state = InitialLoad.GetState();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Play()
        {
            var playerState = PlayerState.GetPlayerState(Session, state.Genie);
            return View(new PLayModel
                {
                    QuestionString = state.Questions[playerState.CurrentQuestionId].Text,
                    Guesses = playerState.AnswerGuesses.Select(g => Tuple.Create(state.Answers[g.AnswerId].Text, g.Probability)).ToList()
                });
        }

        [HttpPost]
        public ActionResult Play(int? choiceId)
        {
            if (choiceId == null)
                return new EmptyResult();
            var playerState = PlayerState.GetPlayerState(Session, state.Genie);
            playerState.AnsweredQuestions.Add(new AnsweredQuestion
                {
                    QuestionId = playerState.CurrentQuestionId, Choise = choiceId.Value
                });
            playerState.AnswerGuesses = state.Genie.GetAnswerGuesses(playerState.AnsweredQuestions);
            playerState.CurrentQuestionId = state.Genie.GetNextQuestionId(playerState.AnsweredQuestions, playerState.AnswerGuesses);
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
            if (answerId == null || !state.Answers.ContainsKey(answerId))
                return new EmptyResult();
            return View(state.Answers[answerId]);
        }
    }
}
