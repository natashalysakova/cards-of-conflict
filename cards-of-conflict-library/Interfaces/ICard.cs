﻿using CardsOfConflict.Library.Enums;
namespace CardsOfConflict.Library.Interfaces;

interface ICard
{
    CardType Type { get; }
    string Text { get; set; }
    Guid ID { get; }
    void SetNewId();
}
