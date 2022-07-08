﻿using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Model;
using System.Collections.ObjectModel;

namespace CardsOfConflict.Library.Game;

internal class RemotePlayer : Player
{
    public RemotePlayer(string name, MessageManager manager) : base(name)
    {
        messageManager = manager;
    }

    private readonly MessageManager messageManager;

    public override void Notify(string text)
    {
        messageManager.SendTextMessage(text);
    }

    public override void SendCards(IEnumerable<WhiteCard> cards)
    {
        messageManager.SendCards(cards);
        Cards.AddRange(cards);
    }

    public override IEnumerable<WhiteCard> GetAnswers(int answersNumber)
    {
        var taken = new List<WhiteCard>();

        messageManager.RequestCards(answersNumber);

        for (int i = 0; i < answersNumber; i++)
        {
            var response = messageManager.GetNextMessage();
            taken.Add(response.Attachment);
        }

        foreach (var card in taken)
        {
            Cards.RemoveAll(x => x.ID == card.ID);
        }
        return taken;
    }

    public override int GetWinner(int answersNumber)
    {
        messageManager.SendWinner(answersNumber);

        var response = messageManager.GetNextMessage();
        return response.Attachment;

    }

    public override void NewRound(int round)
    {
        messageManager.NewRound(round);
    }

    public override void GameOver()
    {
        messageManager.GameOver();
    }

    public override void Stop()
    {
        messageManager.Dispose();
    }

    public override void GameStarted(string[] players)
    {
        messageManager.GameStarted(players);
    }
}
