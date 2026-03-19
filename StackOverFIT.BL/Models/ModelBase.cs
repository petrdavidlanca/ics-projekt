using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StackOverFIT.BL.Models;

public abstract class ModelBase : ObservableObject
{
    public Guid Id { get; set; }
}