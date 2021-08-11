using System.Media;

namespace Frog.Util.WinForm
{
    public class MessageTone
    {
        public enum TriggerType { NONE, Message, Warning, Error };

        public static void Notice(TriggerType beeType = TriggerType.NONE)
        {
            switch (beeType)
            {
                case TriggerType.Message:
                    SystemSounds.Hand.Play();
                    break;
                case TriggerType.Warning:
                    SystemSounds.Beep.Play();
                    break;
                case TriggerType.Error:
                    SystemSounds.Exclamation.Play();
                    break;
                default:
                    break;
            }

        }
    }
}
