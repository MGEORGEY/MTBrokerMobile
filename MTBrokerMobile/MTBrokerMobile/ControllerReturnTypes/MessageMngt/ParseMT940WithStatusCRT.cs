using System.Collections.Generic;

namespace MTBrokerMobile.ControllerReturnTypes.MessageMngt
{
    public class ParseMT940WithStatusCRT
    {
        public SuccessStatusMessageCRT SuccessStatusMessageCRT { get; set; }


        public List<MT940CRT> MT940CRTs { get; set; }
    }
}
