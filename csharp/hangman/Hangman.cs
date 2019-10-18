using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

public class HangmanState
{
    public string MaskedWord { get; }
    public ImmutableHashSet<char> GuessedChars { get; }
    public int RemainingGuesses { get; }

    public HangmanState(string maskedWord, ImmutableHashSet<char> guessedChars, int remainingGuesses)
    {
        MaskedWord = maskedWord;
        GuessedChars = guessedChars;
        RemainingGuesses = remainingGuesses;
    }
}

public class TooManyGuessesException : Exception
{
}

public class Hangman
{
    private ISubject<HangmanState> subj;
    private HangmanState state;
    private IObserver<char> guessObserver;
    
    public IObservable<HangmanState> StateObservable
    {
        get => subj.AsObservable();
    }
    
    public IObserver<char> GuessObserver 
    {
        get => guessObserver;
    }

    public Hangman(string word)
    {
        var maskedWord = String.Join("", word.Select(c => '_'));
        state = new HangmanState(maskedWord, ImmutableHashSet.Create<char>(), 9);
        subj = new BehaviorSubject<HangmanState>(state);
        guessObserver = new Guess(state, subj, word);
    }
}


public class Guess : IObserver<char>
{
    private HangmanState _state;
    private ISubject<HangmanState> _subj;
    private readonly string _word;
    
    public Guess(HangmanState state, ISubject<HangmanState> subj, string word)
    {
        _state = state;
        _subj = subj;
        _word = word;
    } 
    
    void IObserver<char>.OnNext(char value)
    {
        if (!_state.GuessedChars.Contains(value))
        {
            if (_word.Contains(value))
            {
                //var newMask = String.Join("", _state.MaskedWord.Select((c, index) => (_word[index] == value) ? value : _state.MaskedWord[index]));
                var newMask = new StringBuilder(_state.MaskedWord);
                for (var i = 0; i < _word.Length; i++)
                    if (value == _word[i])
                        newMask[i] = value;
                
                
                if (!newMask.ToString().Contains('_'))
                {
                    OnCompleted();
                    _subj.OnCompleted();
                }
                else
                {
                    _state = new HangmanState(newMask.ToString(), _state.GuessedChars.Add(value), _state.RemainingGuesses);
                    _subj.OnNext(_state);
                }
            } 
            else
            {
                FailedGuess(value);
            }
        }
        else
        {
            FailedGuess(value);
        }
    }

    void FailedGuess(char value)
    {
        if (_state.RemainingGuesses == 0)
        {
            _subj.OnError(new TooManyGuessesException());
        }

        if (!_state.GuessedChars.Contains(value))
        {
            _state.GuessedChars.Add(value);
        }

        _state = new HangmanState(_state.MaskedWord, _state.GuessedChars, _state.RemainingGuesses - 1);
       _subj.OnNext(_state);
    }

    void IObserver<char>.OnError(Exception error)
    {
        _subj.OnError(error);
    }

    public void OnCompleted()
    {
        _subj.OnCompleted();
    }
}