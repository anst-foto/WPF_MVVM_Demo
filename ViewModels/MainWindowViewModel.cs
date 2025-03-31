using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using WPF_MVVM_Demo.DataAccess;
using WPF_MVVM_Demo.Models;

namespace WPF_MVVM_Demo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private int? _id;
    public int? Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string? _firstName;
    public string? FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    private string? _lastName;

    public string? LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    private DateTime? _birthDate;

    public DateTime? DateOfBirth
    {
        get => _birthDate;
        set => this.RaiseAndSetIfChanged(ref _birthDate, value);
    }

    public ObservableCollection<User> Users { get; set; } = [];

    private User? _selectedUser;

    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (value == _selectedUser) return;
            
            this.RaiseAndSetIfChanged(ref _selectedUser, value);

            Id = value?.Id;
            LastName = value?.LastName;
            FirstName = value?.FirstName;
            DateOfBirth = value?.DateOfBirth;
        }
    }
    
    public ReactiveCommand<Unit, Unit> CommandLoad { get; }
    public ReactiveCommand<Unit, Unit> CommandSave { get; }
    public ReactiveCommand<Unit, bool> CommandDelete { get; }
    public ReactiveCommand<Unit, Unit> CommandClear { get; }

    public MainWindowViewModel()
    {
        var canExecuteCommandDelete =
            this.WhenAnyValue(
                vm => vm.Id,
                vm => vm.Id,
                (id, _) => id is not null);
        
        var canExecuteCommandSave = this.WhenAnyValue(
            vm => vm.FirstName, 
            vm => vm.LastName, 
            vm => vm.DateOfBirth, 
            (firstName, lastName, dateOfBirth) => !string.IsNullOrEmpty(firstName) &&
                                                  !string.IsNullOrEmpty(lastName) &&
                                                  dateOfBirth is not null);
        var canExecuteCommandClear = this.WhenAnyValue(
            vm => vm.FirstName, 
            vm => vm.LastName, 
            vm => vm.DateOfBirth, 
            (firstName, lastName, dateOfBirth) => !string.IsNullOrEmpty(firstName) ||
                                                  !string.IsNullOrEmpty(lastName) ||
                                                  dateOfBirth is not null);
        
        CommandLoad = ReactiveCommand.CreateFromTask(async () =>
        {
            var users = await DataContext.GetAllUsersAsync();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        });

        CommandDelete = ReactiveCommand.CreateFromTask(
            execute:async () => await DataContext.DeleteUserAsync(Id!.Value),
            canExecute: canExecuteCommandDelete);

        CommandSave = ReactiveCommand.CreateFromTask(
            execute: async () =>
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
            canExecute: canExecuteCommandSave);
        CommandClear = ReactiveCommand.Create(
            execute: () =>
            {
                SelectedUser = null;
                
                LastName = null;
                FirstName = null;
                DateOfBirth = null;
            },
            canExecute: canExecuteCommandClear);
    }
}