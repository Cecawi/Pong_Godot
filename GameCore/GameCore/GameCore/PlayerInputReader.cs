namespace GameCore
{
    public class PlayerInputReader : IPlayerInputReader
    {
        private int _current = 0;

        public void SetIntention(int m)
        {
            _current = m; // -1, 0, 1
        }

        public int ReadIntention()
        {
            return _current;
        }
    }
}