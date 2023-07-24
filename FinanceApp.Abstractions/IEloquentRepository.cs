﻿namespace FinanceApp.Abstractions;

public interface IEloquentRepository<T> where T : Eloquent, new()
{
    public T Find(int id);
    public List<T> All();
}