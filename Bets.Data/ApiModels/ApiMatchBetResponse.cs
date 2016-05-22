using System;
using System.ComponentModel.DataAnnotations;

namespace Bets.Data
{
    public class ApiMatchBetResponse : ActionStatus
    {
        /// <summary>
        /// If bet is successful, contains number of bonus points left for the user, otherwise 0
        /// </summary>
        [Required]
        public new int Result { get; set; }

        public ApiMatchBetResponse(ActionStatus status)
        {
            Success = status.Success;
            Message = status.Message;
        }
    }
}
