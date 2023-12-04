using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.TaskManager
{

    public class TaskManager : ITaskManager
    {
        //private Dictionary<string, Task> executedTasks;
        //private Dictionary<string, Task> executableTasks;
        private LinkedList<Task> taskQueue = new LinkedList<Task>(); // options AddLast & RemoveFirst
        private HashSet<Task> executedTasks = new HashSet<Task>();
        private Dictionary<string, Task> allTasks = new Dictionary<string, Task>();

        public void AddTask(Task task)
        {
            taskQueue.AddLast(task);
            allTasks.Add(task.Id, task);
        }

        public bool Contains(Task task)
        {
            return allTasks.ContainsKey(task.Id);
        }

        public void DeleteTask(string taskId)
        {
            if (!allTasks.ContainsKey(taskId))
            {
                throw new ArgumentException();
            }

            var task = allTasks[taskId];
            allTasks.Remove(taskId);

            if (executedTasks.Contains(task))
            {
                executedTasks.Remove(task);
            }
            else
            {
                taskQueue.Remove(task);
            }
        }

        public Task ExecuteTask()
        {
            if (taskQueue.Count == 0)
            {
                throw new ArgumentException();
            }

            var task = taskQueue.First.Value;

            taskQueue.RemoveFirst();

            executedTasks.Add(task);

            return task;
        }

        public IEnumerable<Task> GetAllTasksOrderedByEETThenByName()
        {
            return this.allTasks.Values
                        .OrderByDescending(t => t.EstimatedExecutionTime)
                        .ThenBy(t => t.Name.Length);
        }

        public IEnumerable<Task> GetDomainTasks(string domain)
        {
            var tasks = taskQueue
                            .Where(t => t.Domain == domain)
                            .ToList();

            if (tasks.Count == 0)
            {
                throw new ArgumentException();
            }

            return tasks;
        }

        public Task GetTask(string taskId)
        {
            if (!allTasks.ContainsKey(taskId))
            {
                throw new ArgumentException();
            }

            return allTasks[taskId];
        }

        public IEnumerable<Task> GetTasksInEETRange(int lowerBound, int upperBound)
        {
            return this.taskQueue
                        .Where(t => t.EstimatedExecutionTime >= lowerBound && t.EstimatedExecutionTime <= upperBound);
        }

        public void RescheduleTask(string taskId)
        {
            if (!allTasks.ContainsKey(taskId))
            {
                throw new ArgumentException();
            }

            var task = allTasks[taskId];
            executedTasks.Remove(task);
            taskQueue.AddLast(task);
        }

        public int Size()
            => this.taskQueue.Count;
    }
}
