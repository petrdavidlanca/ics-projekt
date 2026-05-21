using System.Threading.Tasks;

namespace StackOverFIT.App.Services;

public interface IAlertService
{
    Task DisplayAsync(string title, string message);
}