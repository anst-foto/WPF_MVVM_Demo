using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WPF_MVVM_Demo.DataAccess;
using WPF_MVVM_Demo.Models;

namespace WPF_MVVM_Demo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive] public int? Id {get; set;}
    [Reactive] public string? FirstName{get; set;}
    [Reactive] public string? LastName {get; set;}
    [Reactive] public DateTime? DateOfBirth {get; set;}

    public ObservableCollection<User> Users { get; set; } = [];

    [Reactive] public User? SelectedUser{get; set;}
    
    public ReactiveCommand<Unit, Unit> CommandLoad { get; }
    public ReactiveCommand<Unit, Unit> CommandSave { get; }
    public ReactiveCommand<Unit, bool> CommandDelete { get; }
    public ReactiveCommand<Unit, Unit> CommandClear { get; }

    public MainWindowViewModel()
    {
        this
            .WhenAnyValue(vm => vm.SelectedUser)
            .Subscribe(selectedUser =>
            {
                Id = selectedUser?.Id;
                FirstName = selectedUser?.FirstName;
                LastName = selectedUser?.LastName;
                DateOfBirth = selectedUser?.DateOfBirth;
            });
        
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