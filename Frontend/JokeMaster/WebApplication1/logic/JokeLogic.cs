using WebApplication1.interfaces;

namespace WebApplication1.logic
{
    public class JokeLogic: IJokeLogic
    {

        public JokeLogic()
        {
            indexJoke = 0;
        }

        private int indexJoke;

        public int getJokeindex()
        {
            return indexJoke;
        }

        public bool setJokeIndex(int newIndex)
        {
            try
            {
                indexJoke = newIndex;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
