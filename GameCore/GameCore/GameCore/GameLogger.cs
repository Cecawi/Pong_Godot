namespace GameCore;

public class GameLogger
    {
    private readonly System.Collections.Generic.List<string> _buffer =
        new System.Collections.Generic.List<string>();

    private readonly System.Timers.Timer _timer;

    private readonly GameCore.IGameState _state;
    private readonly GameCore.IPlayerInputReader _input;

    public GameLogger(GameCore.IGameState state, GameCore.IPlayerInputReader input)
    {
        _state = state;
        _input = input;

        _timer = new System.Timers.Timer(50); // 50 ms → 20 Hz
        _timer.Elapsed += OnInterval;
        _timer.Start();
    }

    private void OnInterval(object sender, System.Timers.ElapsedEventArgs e)
    {
        int raw = _input.ReadIntention();  
        var intent = new GameCore.PlayerIntention(raw);

        LogFrame(intent.Move);
    }
    
    public void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }

    private void LogFrame(int action)
    {
        string line =
            _state.BallX + "," + _state.BallY + "," +
            _state.BallVX + "," + _state.BallVY + "," +
            _state.PlayerY + "," + _state.EnemyY + "," +
            action;

        _buffer.Add(line);
    }

    public void SaveCsv(string path)
    {
        System.IO.File.WriteAllLines(path, _buffer.ToArray());
        System.Console.WriteLine("CSV saved at: " + path);
    }
}
