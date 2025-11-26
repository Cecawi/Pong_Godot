namespace GameCore;

public class GameLogger
{
    private readonly System.Collections.Generic.List<string> _buffer =
        new System.Collections.Generic.List<string>();

    private readonly System.Timers.Timer _timer;

    private readonly GameCore.IGameState _state;
    private readonly GameCore.IPlayerInputReader _input1;
    private readonly GameCore.IPlayerInputReader _input2;

    // Constructor now takes 2 input readers
    public GameLogger(GameCore.IGameState state, GameCore.IPlayerInputReader input1, GameCore.IPlayerInputReader input2)
    {
        _state = state;
        _input1 = input1;
        _input2 = input2;

        _timer = new System.Timers.Timer(50); // 50 ms → 20 Hz
        _timer.Elapsed += OnInterval;
        _timer.Start();
    }

    private void OnInterval(object sender, System.Timers.ElapsedEventArgs e)
    {
        int raw1 = _input1.ReadIntention();
        int raw2 = _input2.ReadIntention();
        
        var intent1 = new GameCore.PlayerIntention(raw1);
        var intent2 = new GameCore.PlayerIntention(raw2);

        LogFrame(intent1, intent2);
    }
    
    public void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }

    private void LogFrame(GameCore.PlayerIntention p1Intent, GameCore.PlayerIntention p2Intent)
    {
        var invariant = System.Globalization.CultureInfo.InvariantCulture;
    
        string line = string.Format(invariant,
            "{0},{1},{2},{3},{4},{5},{6},{7}",
            _state.BallX,
            _state.BallY,
            _state.BallVX,
            _state.BallVY,
            _state.PlayerY,
            _state.EnemyY,
            p1Intent.Move,
            p2Intent.Move);

        _buffer.Add(line);
    }

    public void SaveCsv(string path)
    {
        System.IO.File.WriteAllLines(path, _buffer.ToArray());
        System.Console.WriteLine("CSV saved at: " + path);
    }
}