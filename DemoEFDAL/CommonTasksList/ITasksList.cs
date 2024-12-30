using DemoEFBO.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.CommonTasksList
{
    public interface ITasksList
    {
        List<sp_GetTasksForProjects_Result> GetTasks(int projectID, int MainTaskID);

        List<sp_getTaskType_Result> GetTaskType();

        List<sp_getTaskOwners1_Result> GetTaskOwners();

        List<sp_GetTasks_Result> GetTasks(String TaskName);

        bool AddTask(TasksBO TaskObj);

        bool DeleteTask(int ID);

        List<sp_GetTaskOwners_Result> GetTaskOwners(string TaskOwnerName);

        bool AddTaskOwner(ddTaskOwners obj);

        bool DeleteTaskOwner(int id);

        List<sp_GetMainTaskList_Result> GetFilterMainTaskList(string MainTaskName,bool isFilter);

        bool InsertMainTask(MainTaskBO obj, bool isUpdate);

    }
}
