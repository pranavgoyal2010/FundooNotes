﻿namespace RepositoryLayer.CustomException;

public class UserExistsException : Exception
{
    public UserExistsException(string message) : base(message) { }
}
