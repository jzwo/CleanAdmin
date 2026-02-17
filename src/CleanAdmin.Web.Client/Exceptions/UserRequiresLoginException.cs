namespace CleanAdmin.Web.Client.Exceptions;

public class UserRequiresLoginException(string message) : Exception(message);