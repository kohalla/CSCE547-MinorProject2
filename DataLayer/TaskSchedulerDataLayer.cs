using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
	public class TaskSchedulerDataLayer
	{
		/*database context is defined in goals.cs*/

		//get methods for each user
		public List<Task> GetUserTasks(int userId)
		{
			using (var context = new DataLayerContext())
			{
				var tasks = context.Tasks.Where(t => t.UserID == userId).ToList();
				// Do something with the tasks, e.g., return them or process them
				return tasks;
			}
		}
		public Task GetUserTaskbyID(int userId, int taskId)
		{
			using (var context = new DataLayerContext())
			{
				var task = context.Tasks.FirstOrDefault(t => t.UserID == userId && t.TaskID == taskId);
				// Do something with the task, e.g., return it or process it
				return task;
			}
		}
		public Task SearchUserTaskByName(int userId, string taskName)
		{
			using (var context = new DataLayerContext())
			{
				var task = context.Tasks.FirstOrDefault(t => t.UserID == userId && t.Title.Contains(taskName));
				// Do something with the task, e.g., return it or process it
				return task;
			}
		}
		public Task SearchUserTaskbyCategory(int userId, string categoryName)
		{
			using (var context = new DataLayerContext())
			{
				var task = context.Tasks.FirstOrDefault(t => t.UserID == userId && t.Category.CategoryName.Contains(categoryName));
				// Do something with the task, e.g., return it or process it
				return task;
			}
		}
		public List<Task> SearchAllUserTaskbyCategory(int userId, string categoryName)
		{
			using (var context = new DataLayerContext())
			{
				var tasks = context.Tasks.Where(t => t.UserID == userId && t.Category.CategoryGroup.Contains(categoryName)).ToList();
				// Do something with the task, e.g., return it or process it
				return tasks;
			}
		}
		public List<Goal> GetUserGoals(int userId)
		{
			using (var context = new DataLayerContext())
			{
				var goals = context.Goals.Where(g => g.UserID == userId).ToList();
				// Do something with the goals, e.g., return them or process them
				return goals;
			}
		}
		public Goal GetUserGoalbyID(int userId, int goalId)
		{
			using (var context = new DataLayerContext())
			{
				var goal = context.Goals.FirstOrDefault(g => g.UserID == userId && g.GoalID == goalId);
				// Do something with the goal, e.g., return it or process it
				return goal;
			}
		}
		public String GetUsernameByID(int userId)
		{
			using (var context = new DataLayerContext())
			{
				var user = context.Users.FirstOrDefault(a => a.UserID == userId);
				return user?.Username ?? string.Empty;
			}
		}
		public List<TaskCompletion> GetUserTaskCompletions(int userId) {
			using (var context = new DataLayerContext())
			{
				//get all the tasks for the user
				var taskIds = GetUserTasks(userId).Select(t => t.TaskID).ToList();

				//get all of the task completions for that list of task ids
				var userCompletions = context.TaskCompletions.Where(tc => taskIds.Contains(tc.TaskID)).ToList();
				return userCompletions;
			}
		}
		
		public List<Appointment> GetUserAppointments(int userId)
		{
			using (var context = new DataLayerContext())
			{
				var appointments = context.Appointments.Where(a => a.UserID == userId).ToList();
				// Do something with the appointments, e.g., return them or process them
				return appointments;
			}
		}
		public Appointment GetUserAppointmentbyID(int userId, int appointmentId)
		{
			using (var context = new DataLayerContext())
			{
				var appointment = context.Appointments.FirstOrDefault(a => a.UserID == userId && a.AppointmentID == appointmentId);
				// Do something with the appointment, e.g., return it or process it
				return appointment;
			}
		}
		public string GetCategoryNamebyID(int categoryID)
		{
			using (var context = new DataLayerContext())
			{
				var category = context.Categories.FirstOrDefault(a => a.CategoryID == categoryID);
				return category?.CategoryName ?? string.Empty;
			}
		}
		public string GetCategoryGroupbyID(int categoryID)
		{
			using (var context = new DataLayerContext())
			{
				var category = context.Categories.FirstOrDefault(a => a.CategoryID == categoryID);
				if (category == null)
				{
					// Return a sensible default when the category is not found
					return "Uncategorized";
				}
				return category.CategoryGroup;
			}
		}
		public string GetGoalTitlebyID(int goalID)
		{
			using (var context = new DataLayerContext())
			{
				var goal = context.Goals.FirstOrDefault(a => a.GoalID == goalID);
				return goal?.Title ?? string.Empty;
			}
		}
		public List<Category> SearchCategoriesByName(string searchWhat,string searchField)
		{
			using (var context = new DataLayerContext())
			{
				if (searchWhat=="ID")
				{
					if (int.TryParse(searchField, out int categoryId))
					{
						var category = context.Categories.FirstOrDefault(c => c.CategoryID == categoryId);
						return category != null ? new List<Category> { category } : new List<Category>();
					}
					else
					{
						return new List<Category>();
					}
				}
				else if (searchWhat == "Name")
				{
					var categories = context.Categories.Where(c => c.CategoryName.Contains(searchField)).ToList();
					return categories;
				}
				else if (searchWhat == "Group")
				{
					var categories = context.Categories.Where(c => c.CategoryGroup.Contains(searchField)).ToList();
					return categories;
				}
				else
				{
					return new List<Category>();
				}

			}
		}
		public List<Category> GetAllCategories()
		{
			using (var context = new DataLayerContext())
			{
				var categories = context.Categories.ToList();
				return categories;
			}
		}
		public List<int> GetAllAdminUsers()
		{
			using (var context = new DataLayerContext())
			{
				var adminUsers = context.Users.Where(u => u.RoleID == 1).Select(u => u.UserID).ToList();
				return adminUsers;
			}
		}
		//Add methods
		public int AddTask(Task task)
		{
			using (var context = new DataLayerContext())
			{
				context.Tasks.Add(task);
				context.SaveChanges();
				
				return task.TaskID;
			}
		}
		public int AddGoal(Goal goal)
		{
			using (var context = new DataLayerContext())
			{
				context.Goals.Add(goal);
				context.SaveChanges();
				
				return goal.GoalID;
			}
		}
		public int AddAppointment(Appointment appointment)
		{
			using (var context = new DataLayerContext())
			{
				context.Appointments.Add(appointment);
				context.SaveChanges();
				
				return appointment.AppointmentID;
			}
		}
		public void AddTaskCompletion(TaskCompletion completion)
		{
			using (var context = new DataLayerContext())
			{
				context.TaskCompletions.Add(completion);
				context.SaveChanges();
			}
		}

		public void AddCategory(Category category)
		{
			using (var context = new DataLayerContext())
			{
				context.Categories.Add(category);
				context.SaveChanges();
			}
		}
		//update methods
		public void UpdateTaskbyID(int userId, int taskId, Task updatedTask)
		{
			using (var context = new DataLayerContext())
			{
				var task = context.Tasks.FirstOrDefault(t => t.UserID == userId && t.TaskID == taskId);
				if (task != null)
				{
					task.Title = updatedTask.Title;
					task.Description = updatedTask.Description;
					task.Frequency = updatedTask.Frequency;
					task.PreferredDay = updatedTask.PreferredDay;
					task.PreferredTime = updatedTask.PreferredTime;
					task.DurationMinutes = updatedTask.DurationMinutes;
					task.Priority = updatedTask.Priority;
					task.IsFlexible = updatedTask.IsFlexible;
					task.IsActive = updatedTask.IsActive;
					task.CategoryID = updatedTask.CategoryID;
					task.GoalID = updatedTask.GoalID;
					context.SaveChanges();
				}
			}
		}
		public void UpdateGoalbyID(int userId, int goalId, Goal updatedGoal)
		{
			using (var context = new DataLayerContext())
			{
				var goal = context.Goals.FirstOrDefault(g => g.UserID == userId && g.GoalID == goalId);
				if (goal != null)
				{
					goal.CategoryID = updatedGoal.CategoryID;
					goal.Title = updatedGoal.Title;
					goal.Description = updatedGoal.Description;
					goal.StartDate = updatedGoal.StartDate;
					goal.TargetDate = updatedGoal.TargetDate;
					goal.TargetValue = updatedGoal.TargetValue;
					goal.CurrentValue = updatedGoal.CurrentValue;
					goal.Unit = updatedGoal.Unit;
					goal.Status = updatedGoal.Status;
					goal.ProgressPercent = updatedGoal.ProgressPercent;
					context.SaveChanges();
				}
			}
		}
		public void UpdateAppointmentbyID(int userId, int appointmentId, Appointment updatedAppointment)
		{
			using (var context = new DataLayerContext())
			{
				var appointment = context.Appointments.FirstOrDefault(a => a.UserID == userId && a.AppointmentID == appointmentId);
				if (appointment != null)
				{
					appointment.CategoryID = updatedAppointment.CategoryID;
					appointment.Title = updatedAppointment.Title;
					appointment.Location = updatedAppointment.Location;
					appointment.StartDateTime = updatedAppointment.StartDateTime;
					appointment.EndDateTime = updatedAppointment.EndDateTime;
					appointment.Notes = updatedAppointment.Notes;
					appointment.ReminderMinutesBefore = updatedAppointment.ReminderMinutesBefore;
					context.SaveChanges();
				}
			}
		}
		public void UpdateCategory(Category updatedCategory)
		{
			using (var context = new DataLayerContext())
			{
				var category = context.Categories.FirstOrDefault(c => c.CategoryID == updatedCategory.CategoryID);
				if (category != null)
				{
					category.CategoryName = updatedCategory.CategoryName;
					category.CategoryGroup = updatedCategory.CategoryGroup;
					category.Description = updatedCategory.Description;
					category.ColorHex = updatedCategory.ColorHex;
					context.SaveChanges();
				}
			}
		}
		//delete methods
		public void DeleteTaskbyID(int taskId)
		{
			using (var context = new DataLayerContext())
			{
				var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
				if (task != null)
				{
					context.Tasks.Remove(task);
					context.SaveChanges();
				}
			}
		}
		public void DeleteGoalbyID(int goalId)
		{
			using (var context = new DataLayerContext())
			{
				var goal = context.Goals.FirstOrDefault(g => g.GoalID == goalId);
				if (goal != null)
				{
					context.Goals.Remove(goal);
					context.SaveChanges();
				}
			}
		}
		public void DeleteAppointmentbyID(int appointmentId)
		{
			using (var context = new DataLayerContext())
			{
				var appointment = context.Appointments.FirstOrDefault(a => a.AppointmentID == appointmentId);
				if (appointment != null)
				{
					context.Appointments.Remove(appointment);
					context.SaveChanges();
				}
			}
		}
		public void DeleteCategorybyID(int categoryId)
		{
			using (var context = new DataLayerContext())
			{
				var category = context.Categories.FirstOrDefault(c => c.CategoryID == categoryId);
				if (category != null)
				{
					context.Categories.Remove(category);
					context.SaveChanges();
				}
			}
		}
	}

}
