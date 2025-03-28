using System.Collections.ObjectModel;
using System.Windows.Input;
using WPF_MVVM_Demo.DataAccess;
using WPF_MVVM_Demo.Models;

namespace WPF_MVVM_Demo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private int? _id;

    public int? Id
    {
        get => _id;
        set =>
            /*if (value == _id) return;

            _id = value;
            OnPropertyChanged(nameof(Id));*/
            SetField(ref _id, value);
    }

    private string? _firstName;

    public string? FirstName
    {
        get => _firstName;
        set => SetField(ref _firstName, value);
    }

    private string? _lastName;

    public string? LastName
    {
        get => _lastName;
        set => SetField(ref _lastName, value);
    }

    private DateTime? _birthDate;

    public DateTime? DateOfBirth
    {
        get => _birthDate;
        set => SetField(ref _birthDate, value);
    }

    public ObservableCollection<User> Users { get; set; } = [];

    private User? _selectedUser;

    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (!SetField(ref _selectedUser, value)) return;

            Id = value?.Id;
            LastName = value?.LastName;
            FirstName = value?.FirstName;
            DateOfBirth = value?.DateOfBirth;
        }
    }
    
    public ICommand CommandLoad { get; }
    public ICommand CommandSave { get; }
    public ICommand CommandDelete { get; }
    public ICommand CommandClear { get; }

    public MainWindowViewModel()
    {
        CommandLoad = new LambdaCommand(async void (_) =>
        {
            var users = await DataContext.GetAllUsersAsync();
            
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        });

        CommandDelete = new LambdaCommand(
            execute: async void (_) =>
        {
            await DataContext.DeleteUserAsync(Id!.Value);
        },
            canExecute: (_) => Id.HasValue);

        CommandSave = new LambdaCommand(
            execute: async void (_) =>
            {
                if (SelectedUser == null)
                {
                    await DataContext.AddUserAsync(new User()
                    {
                        FirstName = FirstName!,
                        LastName = LastName!,
                        DateOfBirth = DateOfBirth!.Value
                    });
                }
                else
                {
                    await DataContext.UpdateUserAsync(new User()
                    {
                        Id = Id!.Value,
                        FirstName = FirstName!,
                        LastName = LastName!,
                        DateOfBirth = DateOfBirth!.Value
                    });
                }
            },
            canExecute: (_) => !string.IsNullOrEmpty(FirstName) &&
                !string.IsNullOrEmpty(LastName) &&
                DateOfBirth is not null);
        CommandClear = new LambdaCommand(
            execute: (_) =>
            {
                SelectedUser = null;
                
                LastName = null;
                FirstName = null;
                DateOfBirth = null;
            },
            canExecute: (_) => !string.IsNullOrEmpty(FirstName) ||
                               !string.IsNullOrEmpty(LastName) ||
                               DateOfBirth is not null);
    }
}