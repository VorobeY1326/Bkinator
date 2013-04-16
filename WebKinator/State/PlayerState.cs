using System.Collections.Generic;
using System.Web;
using Bkinator;

namespace WebKinator.State
{
    public class PlayerState
    {
        public IList<AnsweredQuestion> AnsweredQuestions { get; set; }
        public IList<AnswerGuess> AnswerGuesses { get; set; }
        public string CurrentQuestionId { get; set; }

        private const string playerStateName = "playerstate";

        public static PlayerState GetPlayerState(HttpSessionStateBase session, Genie genie)
        {
            var state = session[playerStateName];
            if (state == null)
            {
                var answeredQuestions = new List<AnsweredQuestion>();
                var answerGuesses = genie.GetAnswerGuesses(answeredQuestions);
                var currQuestionId = genie.GetNextQuestionId(answeredQuestions, answerGuesses);
                return new PlayerState
                    {
                        AnswerGuesses = answerGuesses,
                        AnsweredQuestions = answeredQuestions,
                        CurrentQuestionId = currQuestionId
                    };
            }
            return (PlayerState) state;
        }

        public static void ResetPlayerState(HttpSessionStateBase session)
        {
            session[playerStateName] = null;
        }
    }
}