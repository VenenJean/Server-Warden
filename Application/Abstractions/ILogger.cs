namespace Application.Abstractions;

public interface ILogger { 
  void LogException(string fileName, string methodName, Exception ex) { }
}