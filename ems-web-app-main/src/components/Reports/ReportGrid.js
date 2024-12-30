import React, { Component } from "react";
import { Dropdown, Row } from "react-bootstrap";
import ReportFilter from "./ReportFilter";
import axios from "axios";
import UpdateAddRecord from "../AddRecord/UpdateAddRecord";
import moment from "moment";
import Loader from "../Loader/Loader";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import Button from "react-bootstrap/Button";
import ButtonGroup from "react-bootstrap/ButtonGroup";
import { NotRefreshReportAction } from "../Redux/Actions/index";
import PopUpModal from "../PopUpMsgModal/PopUpModal";
import Cookies from "js-cookie";
import { connect } from "react-redux";
import Checkbox from "@material-ui/core/Checkbox";
import Tooltip from "@material-ui/core/Tooltip";
import { ThreeSixtySharp } from "@material-ui/icons";
import SureMsgPopUp from "./SureMsgPopUp";
import ConfirmPopModal from "./ConfirmPopUpModal";
import RecordAddedPopUp from "../AddRecord/RecordAddedPopUp";
import RecordUpdatePopUp from "./RecordUpdatePopUp";
import ExportToPDFRefNumber from "./ExportToPDFRefNumber";
import { encrypt, decrypt } from "react-crypt-gsm";
import InfiniteScroll from "react-infinite-scroll-component";
import { toHaveStyle } from "@testing-library/jest-dom";
import CustomColumn from "../ColumnChooser/CustomColumn";
import WBSExcelPopUpModal from "./WBSExcelPopUpModal";
import Exportmodal from "./Exportmodal";

const errorstyle = {
  borderStyle: "solid",
  borderWidth: "2px",
  borderColor: "Red",
};
class ReportGrid extends Component {
  constructor(props) {
    super(props);
    this.state = {
    
      lstReport: null,
      GrouplstReport: null,
      lstProjectNames: null,
      lstProjectNamesAll: null,
      lstMainTask: null,
      lstMaintasksMapped: null,
      ProjectName: "",
      AdminReport: {
      selectedOptions: [],
        ProjectType: "",
        MainTask: null,
        SubTask: null,
        TaskOwner: null,
        ExportType: true,
        Department: null,
        ShowSubTask: false,
     
        value:-1,
      },
      lstSubtask: null,
      lstTaskOwnerName: null,
      lstTaskOwnerName_List: [],
      Days: "",
      StartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
      EndDate: new Date(),
      ShowStartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
      ShowEndDate: new Date(),
      PopUpBit: false,
      FTitle: "Reports Filter",
      AddReportTitle: "Add Assignment",
      page: 0,
      rowsPerPage: 10,
      ReportCount: 0,
      //#region  Report Filter For Staff
      ProjectIdFilter: null,
      //#endregion
      ErrorModel: false,
      DateErrMsg: "",
      ProjectIdFilterShow: "",
      showUpdateModel: false,
      SaveAssignment: true,
      lstAddRecord: null,
      Loading: true,
      HoursDays: "",
      HoursDaysShow: "---Select---",
      UpdateRecordFields: {
        AssignmentID: null,
        ProjectID: null,
        ProjectName: "",
        TaskOwnerName: "",
        TaskOwnerID: null,
        ProjectDescription: "",
        ActualDuration: "",
        Maintask: "",
        Subtask: "",
        SubtaskID: "",
        Comment: "",
        ProjectDate: false,
        APIProjectDate: "",
        MainTaskID: null,
        UpdateCheckBox: false,
        BillableHours: null,
        BillableIsApproved: false,
      },
      errorStyle: {
        ProjectDD: "",
        ResourceNameDD: "",
        ProjectDescription: "",
        ProjectDatefield: "",
        ActualDurationFiled: "",
        PhaseFiled: "",
        DescriptionFiled: "",
        CommentFiled: "",
      },
      sortdatecheck: false,
      sortnamecheck: false,
      ShowZeroHours: false,
      Differencehourse: false,
      ExportIsCheck: false,
      ShowFilterTop: false,
      RecordAddedPopUpBit: false,
      RecordUpdateStatus: false,
      lstClient: [],
      ClientIDSelected: null,
      ProjectHoursList: [],
      IsUpdated: false,
      AdminReportBackup: {
        ProjectType: "",
        MainTask: null,
        SubTask: null,
        TaskOwner: null,
        getvalue: false,
        ClientID: null,
        StartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        EndDate: new Date(),
        ProjectId: null,
        ProjectIdFilterShow: null,
        ShowStartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        ShowEndDate: new Date(),
        IsFilter: false,
        ExportType: true,
        Department: null,
        ShowSubTask: false,
      },
      ExportToPdfModalShow: false,
      pdftextboxvalue: null,
      ColumnChooserModal: false,
      ReportColumnStatus: {
        Project_Name: true,
        Report_Date: true,
        Task_Owner: true,
        Main_Task: true,
        Sub_Task: false,
        Description: true,
        Report_Days: false,
        Report_Duration: true,
        Report_Billable: true,
        Report_Approved: false,
      },
      ReportColumnName: [
        { ColumnName: "Project Name", StateName: "ProjectName", Status: true },
        { ColumnName: "Date", StateName: "AssignmentDateTime", Status: true },
        { ColumnName: "Task Owner", StateName: "UserFullName", Status: true },
        { ColumnName: "Main Task", StateName: "MainTaskName", Status: true },
        { ColumnName: "Sub Task", StateName: "TaskName", Status: false },
        { ColumnName: "Description", StateName: "CommentText", Status: true },
        { ColumnName: "Days", StateName: "DaysDuration", Status: true },
        {
          ColumnName: "Duration (Hours) ",
          StateName: "ActualDuration",
          Status: true,
        },
        {
          ColumnName: "Billable (Hours)",
          StateName: "BillableHours",
          Status: true,
        },
        {
          ColumnName: "Approved",
          StateName: "IsBillableApproved",
          Status: false,
        },
      ],
      ShowColumnChooser: false,
      GroupByProject: [],
      lstDepartmentNames: [],
      WBSExcelShow: false,
      WBSExcelProjectID: -1,
      WBSExcelProjectName: null,
      ExportShow: false,
      Bill_Actual: null,
      Export_Type: null,
      Export_Media: null,
    };
  }

  componentDidMount() {
    
    const GetKey = Cookies.get("encrypted_Type");
    const Getbytes = JSON.parse(Cookies.get("encrypted_Type_Length"));
    const GetEncryptedFormat = { content: GetKey, tag: Getbytes.data };
    const decryptValue = decrypt(GetEncryptedFormat);
    const CookieValue = Cookies.get("Role");
    if (
      decryptValue != CookieValue &&
      (decryptValue != "Staff" ||
        decryptValue != "Admin" ||
        decryptValue != "SuperAdmin")
    ) {
      this.logout();
    }
    this.LoadReportsData();
    this.LoadProjectsName();
    this.LoadMainTasks();
    this.LoadTasksOwnerName();
    this.LoadClients();
    this.LoadProjectsHours();
    this.LoadDepartment();
  }
  //#region API Calls
  logout = () => {
    try {
      localStorage.removeItem("token");
      localStorage.removeItem("Video_Upload_Size");
      localStorage.removeItem("S3Obj");
      localStorage.removeItem("userId");
      localStorage.removeItem("EncryptedType");
      localStorage.removeItem("access_token");
      localStorage.removeItem("login_time");
      localStorage.removeItem("expires_in");
      Cookies.remove("Role");
      Cookies.remove("UserID");
      Cookies.remove("UserName");
      Cookies.remove("Design");
      Cookies.remove("encrypted_Type");
      Cookies.remove("encrypted_Type_Length");
      window.location.href = ".";
    } catch (ee) {
      alert(ee);
    }
  };
  LoadReportsData = () => {
    try {
      var loggin=[];
      loggin= [Cookies.get("Role") === "Staff" ? Cookies.get("UserID") : -1]
      this.setState({ ShowFilterTop: true });
      this.setState({ Loading: true, GrouplstReport: [] });
      const objRptParam = {
        FromDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        IsApproved: null,
        LoggedInUserId:
        loggin,
        MainTaskID: -1,
        ProjectId:
          this.state.ProjectIdFilter === null ||
          this.state.ProjectIdFilter === ""
            ? -1
            : this.state.ProjectIdFilter, //"All",
        SubTaskID: -1,
        ToDate: "",
        ClientID: this.state.ClientIDSelected,
        DepartmentID: -1,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/GetAssignmentsGrid`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: objRptParam,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        var roletype = Cookies.get("Role");
        var _tempCount = 0;
        var tempList = [];
        if (res.data.Result !== null) {
          res.data.Result.map((item) => {
            if (item.ProjectName === "TOTAL") {
              _tempCount = _tempCount + 0;
            } else {
              _tempCount++;
              tempList.push(item);
            }
          });
        }
        if (roletype == "Staff") {
          var getlist = tempList.filter((Name) => Name.AssignmentID > 0);
          tempList = [];
          tempList = getlist;
          _tempCount = getlist.length;
        }
        console.log(tempList);
        // for (var i = 0; i <= 2; i++) {
        //   tempList.sort(function (a, b) {
        //     a = a.AssignmentDateTime.split("/");
        //     /* a[0] = day
        //     a[1] = month
        //     a[2] = year */
        //     var _tempa = new Date(a[2], a[1], a[0]);
        //     b = b.AssignmentDateTime.split("/");
        //     var _tempb = new Date(b[2], b[1], b[0]);
        //     return _tempb - _tempa;
        //   });
        // }
        this.setState({ lstReport: tempList });
        this.setState({ ReportCount: _tempCount });
        this.setState({ Loading: false });
        var grouparray = [];
        for (var i = 0; i < res.data.Result.length; i++) {
          if (grouparray.length == 0) {
            var obj = {
              ProjectNameAll: res.data.Result[i]?.ProjectName,
              ProjectIdAll: res.data.Result[i]?.ProjectID,
              ReferenceNumber: null,
              ClientID: res.data.Result[i]?.ClientID,
            };
            grouparray.push(obj);
          } else {
            var CheckProject = grouparray.filter(
              (SName) => SName.ProjectIdAll === res.data.Result[i]?.ProjectID
            );
            if (CheckProject.length == 0) {
              var obj = {
                ProjectNameAll: res.data.Result[i]?.ProjectName,
                ProjectIdAll: res.data.Result[i]?.ProjectID,
                ReferenceNumber: null,
                ClientID: res.data.Result[i]?.ClientID,
              };
              grouparray.push(obj);
            }
          }
        }
        if (grouparray.length > 0) {
          grouparray = grouparray.filter((Name) => Name.ProjectIdAll != null);
        }

        const result = grouparray.reduce((acc, d) => {
          const found = acc.find((a) => a.ClientID === d.ClientID);

          const clientname = this.state.lstClient.find(
            (a) => a.ID === d.ClientID
          );

          //const value = { name: d.name, val: d.value };
          const value = {
            ClientID: d.ClientID,
            value:
              clientname == undefined
                ? d.ProjectNameAll
                : clientname.ClientName,
            count: d.count,
          }; // the element in data property
          if (!found) {
            //acc.push(...value);
            acc.push({
              ClientID: d.ClientID > 0 ? d.ClientID : null,
              name:
                clientname == undefined
                  ? d.ProjectNameAll
                  : clientname.ClientName,
              data: [d],
            }); // not found, so need to add data property
          } else {
            //acc.push({ name: d.name, data: [{ value: d.value }, { count: d.count }] });
            found.data.push(value); // if found, that means data property exists, so just push new element to found.data.
          }
          return acc;
        }, []);

        this.setState({ GrouplstReport: grouparray, GroupByProject: result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadDepartment = () => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Department/GetDepartments`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        var getDepartment = res.data.Result.filter(
          (Name) => Name.IsActive == true
        );
        this.setState({
          lstDepartmentNames: getDepartment,
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadProjectsName = () => {
    try {
      const ProjectcObj = {
        ProjectName: "",
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjects`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectcObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstProjectNames: res.data.Result,
          lstProjectNamesAll: res.data.Result,
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadClients = () => {
    try {
      this.setState({ Loading: true });
      var Payload = {
        ID: 0,
        ClientName: "",
        ClientEmail: "",
        ContactNumber: "",
        Address: "",
        Website_URL: "",
        Facebooklink: "",
        Twitter: "",
        instagramlink: "",
        Active: 0,
      };

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Client/GetClientList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: Payload,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstClient: res.data.Result,
        });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadMainTasks = () => {
    try {
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMainTask: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  LoadMainTasksMapped = () => {
    try {
      const ProjectId = {
        ProjectID: this.state.UpdateRecordFields.ProjectID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTaskMappedList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectId,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMaintasksMapped: res.data.Result }, () => {
          if (this.state.showUpdateModel == true) {
            //need to do this for update case...when we select project field maintask select should update
            this.setState(
              {
                UpdateRecordFields: {
                  ...this.state.UpdateRecordFields,
                  Maintask: this.state.lstMaintasksMapped[0]?.MainTaskName,
                  MainTaskID: this.state.lstMaintasksMapped[0]?.Id,
                },
              },
              () => {
                this.LoadSubTasks(
                  this.state.UpdateRecordFields.ProjectID,
                  this.state.UpdateRecordFields.MainTaskID
                );
              }
            );
          }
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadFirstMainTasksMapped = () => {
    try {
      const ProjectId = {
        ProjectID: this.state.UpdateRecordFields.ProjectID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTaskMappedList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectId,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMaintasksMapped: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadTasksOwnerName = () => {
    try {
      const UserIdObj = {
        UserID: 0,
        DepartID: this.state.AdminReport.Department,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/GetUserProfile`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserIdObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        var ArrayList =[];
        ArrayList.push({ label: "ALL", value: -1 });
        for(var i=0;i<res.data.Result.length;i++){
          const obj={label:res.data.Result[i]?.Name,value:res.data.Result[i]?.UserProfileTableID}
          ArrayList.push(obj)
        }
        this.setState({lstTaskOwnerName_List:ArrayList},()=>{
          
          this.setSelectedOptions(this.state.lstTaskOwnerName_List);
        }
        )
        this.setState({ lstTaskOwnerName: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadFirstSubTasks = (ProjectName, MainTaskName) => {
    try {
      const userObj = {
        projectID: ProjectName,
        maintaskID: MainTaskName,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}SubTasks/GetSubTaskList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: userObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstSubtask: res.data.Result });
      });
    } catch (e) {
      alert(e);
    }
  };
  /*  LoadTasksOwnerName = () => {
    try {
      const TaskOwnersObj = {
        TaskOwnerNames: "",
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}TaskOwners/GetTaskOwners`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: TaskOwnersObj,
      }).then((res) => {
        this.setState({ lstTaskOwnerName: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  }; */
  LoadSubTasks = (ProjectName, MainTaskName) => {
    try {
      const userObj = {
        projectID: ProjectName,
        maintaskID: MainTaskName,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}SubTasks/GetSubTaskList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: userObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstSubtask: res.data.Result }, () => {
          if (this.state.showUpdateModel == true) {
            //need to do this for update case...when we select project field maintask select should update
            this.setState({
              UpdateRecordFields: {
                ...this.state.UpdateRecordFields,
                Subtask: this.state.lstSubtask[0]?.TaskName,
                SubtaskID: this.state.lstSubtask[0]?.SubtaskId,
              },
            });
          }
        });
        if (res.data.Result.length == 0) {
          this.setState({
            AdminReport: {
              ...this.state.AdminReport,
              SubTask: "",
            },
          });
        }
      });
    } catch (e) {
      alert(e);
    }
  };
  CheckPopUpUpdate = () => {
    this.setState({ RecordUpdatePopUpBit: false });
  };
  CloseModalAfterUpdate = () => {
    this.setState({ RecordUpdatePopUpBit: false });
  };

  UpdateCheckValidation = (e) => {
    var checkError = false;

    if (this.state.UpdateRecordFields.ActualDuration === "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ActualDurationFiled: "input_error",
        },
      }));
    }
    if (this.state.UpdateRecordFields.APIProjectDate === "Invalid date") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectDatefield: "input_error",
        },
      }));
    }
    if (this.state.UpdateRecordFields.UpdateCheckBox !== true) {
      if (parseFloat(this.state.UpdateRecordFields.ActualDuration) === 0) {
        checkError = true;
        this.setState({ Title: "Record Update" });
        this.setState({ errorMsgs: "Actual Duration cannot be equal to zero" });

        this.setState({ RecordUpdatePopUpBit: true });
        e.preventDefault();
      }
    }
    if (parseFloat(this.state.UpdateRecordFields.ActualDuration < 0)) {
      checkError = true;
      this.setState({ Title: "Record Update" });
      this.setState({
        errorMsgs: "Actual Duration cannot be less then zero",
      });

      this.setState({ RecordUpdatePopUpBit: true });
      e.preventDefault();
    }
    if (parseFloat(this.state.UpdateRecordFields.ActualDuration) > 24) {
      checkError = true;
      this.setState({ Title: "Record Update" });
      this.setState({
        errorMsgs: "Actual Duration cannot be greater then 24 Hours",
      });

      this.setState({ RecordUpdatePopUpBit: true });
      e.preventDefault();
    }

    if (this.state.UpdateRecordFields.Comment === "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          CommentFiled: "input_error",
        },
      }));
    }

    if (!checkError) {
      this.setState({ Title: "Update Record" });
      this.setState({
        errorMsg:
          "Are you sure you want to update " +
          this.state.UpdateRecordFields.ActualDuration +
          " Hour(s) in " +
          this.state.UpdateRecordFields.ProjectName +
          "?",
      });
      this.setState({ checkPopUp: "save" });
      this.setState({
        ConfrimMsgPopUp: true,
        RecordUpdateStatus: true,
        IsUpdated: true,
      });
    }
    e.preventDefault();
  };
  // UpdateStyleRemoved = () => {
  //   this.setState({ errorStyle: {} });
  // };
  /* UpdateRecords = (e) => {
    try {
      var RequestObj = {
        ID: this.state.UpdateRecordFields.AssignmentID,
        ProjectName: this.state.UpdateRecordFields.ProjectName,
        TaskName: this.state.UpdateRecordFields.Subtask,
        TaskOwnerName: this.state.UpdateRecordFields.TaskOwnerName,
        AssignmentDateTime: this.state.UpdateRecordFields.APIProjectDate,
        ActualDuration: this.state.UpdateRecordFields.ActualDuration,
        Type: "Digital Marketing", // it's hardcoded in ems .net need to discuss it with faisal bhai
        SubTaskName: this.state.UpdateRecordFields.Comment,
        TaskType: "N", // hardcoded later we will change
        MainTaskID: this.state.UpdateRecordFields.MainTaskID,
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/SaveAssignments`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: RequestObj,
      }).then((res) => {
        this.CloseModal();
        this.refreshGrid();
      });
    } catch (ex) {
      window.location.href = "/Error";
      //alert(ex);
    }
  }; */
  CloseReportModal = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CheckPopUp = () => {
    this.DeleteRecord();
  };
  ConfrimModal = () => {
    this.setState({ Title: "Delete Record" });
    this.setState({ errorMsg: "Are you sure you want to Delete " });
    this.setState({ ShowMsgPopUp: true });
  };
  DeleteRecord = (AssignmentID) => {
    try {
      const DelAssign = {
        ID: this.state.UpdateRecordFields.AssignmentID,
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/DeleteAssignment`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DelAssign,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ ShowMsgPopUp: false });
        this.CloseModal();
        this.RefreshGridData();
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  // ConfrimModales = () => {
  //   this.setState({ Title: "Update  Record" });
  //   this.setState({
  //     errorMsg:
  //       "Are you sure you want to update " +
  //       this.state.UpdateRecordFields.ActualDuration +
  //       " Hour(s) in " +
  //       this.state.UpdateRecordFields.ProjectName +
  //       "?",
  //   });
  //   this.setState({ checkPopUp: "save" });
  //   this.setState({ ConfrimMsgPopUp: true, RecordUpdateStatus: true });
  // };

  UpdateRecords = (e) => {
    try {

      // var RequestObj = {
      //   ID: 0, // it's 0 for save case
      //   ProjectID: this.state.ProjectId,
      //   TASKID: this.state.TaskID,
      //   UserID:
      //     this.state.TaskOwnerID != null
      //       ? this.state.TaskOwnerID
      //       : Cookies.get("UserID"), //Sending UserID gET TaskOwnerID In SP
      //   AssignmentDateTime: this.props.AssignmentDateTime,
      //   ActualDuration: this.state.ActualDuration,
      //   CommentText: this.state.Comment,
      //   MainTaskID: this.state.MainTaskID,
      // };
      var RequestObj;
      //Need UserId AND Billable Hours in GetAssignemnt
      if (this.state.UpdateRecordFields.UpdateCheckBox == true) {
        RequestObj = {
          ID: this.state.UpdateRecordFields.AssignmentID,
          ProjectID: this.state.UpdateRecordFields.ProjectID,
          MainTaskID: this.state.UpdateRecordFields.MainTaskID,
          TASKID: this.state.UpdateRecordFields.SubtaskID,
          UserID: this.state.UpdateRecordFields.TaskOwnerID,
          AssignmentDateTime: moment(
            this.state.UpdateRecordFields.APIProjectDate
          ).format("MM/DD/YYYY"),
          CommentText: this.state.UpdateRecordFields.Comment,
          BillableHours: this.state.UpdateRecordFields.ActualDuration,
          IsBillableApproved: 0,
          ActualDuration: null,
        };
      } else {
        RequestObj = {
          ID: this.state.UpdateRecordFields.AssignmentID,
          ProjectID: this.state.UpdateRecordFields.ProjectID,
          MainTaskID: this.state.UpdateRecordFields.MainTaskID,
          TASKID: this.state.UpdateRecordFields.SubtaskID,
          UserID: this.state.UpdateRecordFields.TaskOwnerID,
          AssignmentDateTime: moment(
            this.state.UpdateRecordFields.APIProjectDate
          ).format("MM/DD/YYYY"),
          CommentText: this.state.UpdateRecordFields.Comment,
          ActualDuration: this.state.UpdateRecordFields.ActualDuration,
          IsActualApproved: 0,
          BillableHours: null,
        };
      }
      // if (this.state.SaveAssignment) {
      console.log(RequestObj);
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/SaveAssignments`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: RequestObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ Title: "Success" });
        this.setState({ errorMsg: "Record Update Successfully" });
        this.setState({ RecordAddedPopUpBit: true });
        // this.setState({ showSelectedDate: false, IsUpdated: true });
        //this.CloseModal();
        // this.refreshGrid();
        //this.RefreshGridData();
      });
      // }
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  CloseModalAfterAdd = () => {
    this.setState(
      {
        RecordAddedPopUpBit: false,
      },
      () => {
        this.CloseModal();
        this.RefreshGridData();
      }
    );
  };
  UpdateRecordsApproved = (e) => {
    try {
      var RequestObj;
      if (this.state.UpdateRecordFields.UpdateCheckBox == true) {
        RequestObj = {
          ID: this.state.UpdateRecordFields.AssignmentID,
          ProjectID: this.state.UpdateRecordFields.ProjectID,
          MainTaskID: this.state.UpdateRecordFields.MainTaskID,
          TASKID: this.state.UpdateRecordFields.SubtaskID,
          UserID: this.state.UpdateRecordFields.TaskOwnerID,
          AssignmentDateTime: this.state.UpdateRecordFields.APIProjectDate,
          CommentText: this.state.UpdateRecordFields.Comment,
          BillableHours: this.state.UpdateRecordFields.ActualDuration,
          IsBillableApproved: 1,
          UserIDBillable: this.state.UpdateRecordFields.TaskOwnerID,
          ActualDuration: null,
        };
      } else {
        RequestObj = {
          ID: this.state.UpdateRecordFields.AssignmentID,
          ProjectID: this.state.UpdateRecordFields.ProjectID,
          MainTaskID: this.state.UpdateRecordFields.MainTaskID,
          TASKID: this.state.UpdateRecordFields.SubtaskID,
          UserID: this.state.UpdateRecordFields.TaskOwnerID,
          AssignmentDateTime: this.state.UpdateRecordFields.APIProjectDate,
          ActualDuration: this.state.UpdateRecordFields.ActualDuration,
          CommentText: this.state.UpdateRecordFields.Comment,
          IsActualApproved: 1,
          UserIDActual: this.state.UpdateRecordFields.TaskOwnerID,
          BillableHours: null,
        };
      }
      if (this.state.SaveAssignment) {
        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/SaveAssignments`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: RequestObj,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          this.setState({ Title: "Success" });
          this.setState({ errorMsg: "Record Approved Successfully" });
          this.setState({ RecordAddedPopUpBit: true });
          this.setState({ showSelectedDate: false });
          // this.CloseModal();
          // this.RefreshGridData();
          // this.refreshGrid();
        });
      }
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  // onExportExcelPDF = () => {
  //   if (this.state.Bill_Actual.toLowerCase() == "Billable".toLowerCase()) {
  //   }
  // };

  ExportExcelReportActualHrs = () => {
    try {
      this.setState({ Loading: true });
      // var FileName = "";
      //const ExportAssignment = null;
      // var asdas = this.state.GrouplstReport.map(({
      //   ProjectID
      // }) => ProjectID).join(', ');
      // var asd = this.state.GrouplstReport?.ProjectID?.join(", ")
      if (
        this.state.ProjectIdFilter == null ||
        this.state.ProjectIdFilter == ""
      ) {
        // var AllReports = this.state.GrouplstReport;
        // for (var i = 0; i < AllReports.length; i++) {

        var FileName =
          "_" +
          moment(this.state.StartDate).format("DD_MM_YYYY") +
          "-" +
          moment(this.state.EndDate).format("DD_MM_YYYY");
        const ExportAssignment = {
          ProjectId: 0,
          TaskOwnerId:
            this.state.AdminReport.TaskOwner != null
              ? this.state.AdminReport.TaskOwner
              : "All",
          FromDate: this.state.StartDate,
          ToDate: this.state.EndDate,
          TaskId:
            this.state.AdminReport.MainTask == null
              ? -1
              : this.state.AdminReport.MainTask == ""
              ? -1
              : this.state.AdminReport.MainTask,
          SubTaskId:
            this.state.AdminReport.SubTask == null
              ? -1
              : this.state.AdminReport.SubTask == ""
              ? -1
              : this.state.AdminReport.SubTask,
          Billable: this.state.ExportIsCheck == true ? 1 : 0,
          IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
          IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
          SettingDateRange: 0,
          fileName: FileName,
          AllArrays: this.state.GrouplstReport,
          pdftextboxvalue: this.state.pdftextboxvalue,
          ExportType: this.state.AdminReport.ExportType,
          ClientArray: this.state.lstClient,
          DepartmentID:
            this.state.AdminReport.Department == null
              ? -1
              : this.state.AdminReport.Department === ""
              ? -1
              : this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        };

        axios({
          method: "Post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToExcelctualHrsWeeklyAll`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: ExportAssignment,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode == 409) {
            //409 = conflict on server
            this.setState({
              ErrorModel: true,
              DateErrMsg: res.data.Result,
            });
          } else if (res.data.Result == "Failed") {
            this.setState({
              ErrorModel: true,
              DateErrMsg: "No Record Avaliable",
            });
          } else {
            for (var i = 0; i < res.data.Result.length; i++) {
              window.open(
                `http://localhost:1316/ExcelFiles/${res.data.Result[i]}`,
                "_blank"
              );
            }
          }
          this.setState({ Loading: false });
        });
        // }
      } else {
        // var FileName =
        //   this.state.ProjectIdFilterShow + "_" + moment().format("DD-MM-YYYY");YYYYMMDD
        var FileName =
          this.state.ProjectIdFilterShow +
          "_" +
          moment(this.state.StartDate).format("DD_MM_YYYY") +
          "-" +
          moment(this.state.EndDate).format("DD_MM_YYYY");
        const ExportAssignment = {
          ProjectId: this.state.ProjectIdFilter,
          TaskOwnerId:
            this.state.AdminReport.TaskOwner != null
              ? this.state.AdminReport.TaskOwner
              : "All",
          FromDate: this.state.StartDate,
          ToDate: this.state.EndDate,
          TaskId:
            this.state.AdminReport.MainTask == null
              ? -1
              : this.state.AdminReport.MainTask == ""
              ? -1
              : this.state.AdminReport.MainTask,
          SubTaskId:
            this.state.AdminReport.SubTask == null
              ? -1
              : this.state.AdminReport.SubTask == ""
              ? -1
              : this.state.AdminReport.SubTask,
          Billable: this.state.ExportIsCheck == true ? 1 : 0,
          IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
          IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
          SettingDateRange: 0,
          fileName: FileName,
          pdftextboxvalue: this.state.pdftextboxvalue,
          ExportType: this.state.AdminReport.ExportType,
          ClientArray: this.state.lstClient,
          DepartmentID:
            this.state.AdminReport.Department == null
              ? -1
              : this.state.AdminReport.Department == ""
              ? -1
              : this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        };

        axios({
          method: "Post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToExcelWeeklyActualHrs`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: ExportAssignment,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode == 409) {
            //409 = conflict on server
            this.setState({
              ErrorModel: true,
              DateErrMsg: res.data.Result,
            });
          } else if (res.data.Result == "Failed") {
            this.setState({
              ErrorModel: true,
              DateErrMsg: "No Record Avaliable",
            });
          } else {
            window.open(
              `http://localhost:1316/ExcelFiles/${res.data.File_Name}`,
              "_blank"
            );
          }
          this.setState({ Loading: false });
          //this.DeleteFiles();
        });
      }
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ExportExcelReport = () => {
    try {
      this.setState({ Loading: true });
      // var FileName = "";
      //const ExportAssignment = null;
      // var asdas = this.state.GrouplstReport.map(({
      //   ProjectID
      // }) => ProjectID).join(', ');
      // var asd = this.state.GrouplstReport?.ProjectID?.join(", ")
      if (
        this.state.ProjectIdFilter == null ||
        this.state.ProjectIdFilter == ""
      ) {
        // var AllReports = this.state.GrouplstReport;
        // for (var i = 0; i < AllReports.length; i++) {

        var FileName =
          "_" +
          moment(this.state.StartDate).format("DD_MM_YYYY") +
          "-" +
          moment(this.state.EndDate).format("DD_MM_YYYY");

        const ExportAssignment = {
          ProjectId: 0,
          TaskOwnerId:
            this.state.AdminReport.TaskOwner != null
              ? this.state.AdminReport.TaskOwner
              : "All",
          FromDate: this.state.StartDate,
          ToDate: this.state.EndDate,
          TaskId:
            this.state.AdminReport.MainTask == null
              ? -1
              : this.state.AdminReport.MainTask == ""
              ? -1
              : this.state.AdminReport.MainTask,
          SubTaskId:
            this.state.AdminReport.SubTask == null
              ? -1
              : this.state.AdminReport.SubTask == ""
              ? -1
              : this.state.AdminReport.SubTask,
          Billable: this.state.ExportIsCheck == true ? 1 : 0,
          IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
          IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
          SettingDateRange: 0,
          fileName: FileName,
          AllArrays: this.state.GrouplstReport,
          pdftextboxvalue: this.state.pdftextboxvalue,
          ExportType: this.state.AdminReport.ExportType,
          ClientArray: this.state.lstClient,
          DepartmentID:
            this.state.AdminReport.Department == null
              ? -1
              : this.state.AdminReport.Department == ""
              ? -1
              : this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        };

        axios({
          method: "Post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToExcelWeeklyAll`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: ExportAssignment,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode == 409) {
            //409 = conflict on server
            this.setState({
              ErrorModel: true,
              DateErrMsg: res.data.Result,
            });
          } else if (res.data.Result == "Failed") {
            this.setState({
              ErrorModel: true,
              DateErrMsg: "No Record Avaliable",
            });
          } else {
            for (var i = 0; i < res.data.Result.length; i++) {
              window.open(
                `http://localhost:1316/ExcelFiles/${res.data.Result[i]}`,
                "_blank"
              );
            }
          }
          this.setState({ Loading: false });
        });
        // }
      } else {
        // var FileName =
        //   this.state.ProjectIdFilterShow + "_" + moment().format("DD-MM-YYYY");YYYYMMDD
        var FileName =
          this.state.ProjectIdFilterShow +
          "_" +
          moment(this.state.StartDate).format("DD_MM_YYYY") +
          "-" +
          moment(this.state.EndDate).format("DD_MM_YYYY");
        const ExportAssignment = {
          ProjectId: this.state.ProjectIdFilter,
          TaskOwnerId:
            this.state.AdminReport.TaskOwner != null
              ? this.state.AdminReport.TaskOwner
              : "All",
          FromDate: this.state.StartDate,
          ToDate: this.state.EndDate,
          TaskId:
            this.state.AdminReport.MainTask == null
              ? -1
              : this.state.AdminReport.MainTask == ""
              ? -1
              : this.state.AdminReport.MainTask,
          SubTaskId:
            this.state.AdminReport.SubTask == null
              ? -1
              : this.state.AdminReport.SubTask == ""
              ? -1
              : this.state.AdminReport.SubTask,
          Billable: this.state.ExportIsCheck == true ? 1 : 0,
          IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
          IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
          SettingDateRange: 0,
          fileName: FileName,
          pdftextboxvalue: this.state.pdftextboxvalue,
          ExportType: this.state.AdminReport.ExportType,
          ClientArray: this.state.lstClient,
          DepartmentID:
            this.state.AdminReport.Department == null
              ? -1
              : this.state.AdminReport.Department == ""
              ? -1
              : this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        };

        axios({
          method: "Post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToExcelWeekly`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: ExportAssignment,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode == 409) {
            //409 = conflict on server
            this.setState({
              ErrorModel: true,
              DateErrMsg: res.data.Result,
            });
          } else if (res.data.Result == "Failed") {
            this.setState({
              ErrorModel: true,
              DateErrMsg: "No Record Avaliable",
            });
          } else {
            window.open(
              `http://localhost:1316/ExcelFiles/${res.data.File_Name}`,
              "_blank"
            );
          }
          this.setState({ Loading: false });
          //this.DeleteFiles();
        });
      }
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ExportPdfReport = () => {
    debugger
    try {
      this.setState({ Loading: true });
      if (
        this.state.ProjectIdFilter == null ||
        this.state.ProjectIdFilter == ""
      ) {
        // var AllReports = this.state.GrouplstReport;
        // for (var i = 0; i < AllReports.length; i++) {

        var FileName =
          "_" +
          moment(this.state.StartDate).format("DD_MM_YYYY") +
          "-" +
          moment(this.state.EndDate).format("DD_MM_YYYY");

        const ExportAssignment = {
          ProjectId: 0,
          TaskOwnerId:
            this.state.AdminReport.TaskOwner != null
              ? this.state.AdminReport.TaskOwner
              : "All",
          FromDate: this.state.StartDate,
          ToDate: this.state.EndDate,
          TaskId:
            this.state.AdminReport.MainTask == null
              ? -1
              : this.state.AdminReport.MainTask == ""
              ? -1
              : this.state.AdminReport.MainTask,
          SubTaskId:
            this.state.AdminReport.SubTask == null
              ? -1
              : this.state.AdminReport.SubTask == ""
              ? -1
              : this.state.AdminReport.SubTask,
          Billable: this.state.ExportIsCheck == true ? 1 : 0,
          IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
          IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
          SettingDateRange: 0,
          fileName: FileName,
          AllArrays: this.state.GrouplstReport,
          pdftextboxvalue: this.state.pdftextboxvalue,
          ExportType: this.state.AdminReport.ExportType,
          ClientArray: this.state.lstClient,
          DepartmentID:
            this.state.AdminReport.Department == null
              ? -1
              : this.state.AdminReport.Department == ""
              ? -1
              : this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        };

        axios({
          method: "Post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToPdfWeeklyAll`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: ExportAssignment,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode == 409) {
            //409 = conflict on server
            this.setState({
              ErrorModel: true,
              DateErrMsg: res.data.Result,
            });
          } else if (res.data.Result == "Failed") {
            this.setState({
              ErrorModel: true,
              DateErrMsg: "No Record Avaliable",
            });
          } else {
            for (var i = 0; i < res.data.Result.length; i++) {
              window.open(
                `http://localhost:1316/PdfFiles/${res.data.Result[i]}`,
                "_blank"
              );
            }
          }
          this.setState({ Loading: false });
        });
        // }
      } else {
        var FileName =
          this.state.ProjectIdFilterShow +
          "_" +
          moment(this.state.StartDate).format("DD_MM_YYYY") +
          "-" +
          moment(this.state.EndDate).format("DD_MM_YYYY");
        const ExportAssignment = {
          ProjectId: this.state.ProjectIdFilter,
          TaskOwnerId:
            this.state.AdminReport.TaskOwner != null
              ? this.state.AdminReport.TaskOwner
              : "All",
          FromDate: this.state.StartDate,
          ToDate: this.state.EndDate,
          TaskId:
            this.state.AdminReport.MainTask == null
              ? -1
              : this.state.AdminReport.MainTask == ""
              ? -1
              : this.state.AdminReport.MainTask,
          SubTaskId:
            this.state.AdminReport.SubTask == null
              ? -1
              : this.state.AdminReport.SubTask == ""
              ? -1
              : this.state.AdminReport.SubTask,
          Billable: this.state.ExportIsCheck == true ? 1 : 0,
          IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
          IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
          SettingDateRange: 0,
          fileName: FileName,
          AllArrays: this.state.GrouplstReport,
          pdftextboxvalue: this.state.pdftextboxvalue,
          ExportType: this.state.AdminReport.ExportType,
          ClientArray: this.state.lstClient,
          DepartmentID:
            this.state.AdminReport.Department == null
              ? -1
              : this.state.AdminReport.Department == ""
              ? -1
              : this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        };
        axios({
          method: "Post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToPdfWeekly`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: ExportAssignment,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode == 409) {
            //409 = conflict on server
            this.setState({
              ErrorModel: true,
              DateErrMsg: res.data.Result,
            });
          } else if (res.data.Result == "Failed") {
            this.setState({
              ErrorModel: true,
              DateErrMsg: "No Record Avaliable",
            });
          } else {
            window.open(
              `http://localhost:1316/PdfFiles/${res.data.File_Name}`,
              "_blank"
            );
          }
          this.setState({ Loading: false });
        });
      }
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  //#endregion

  //#region Click events

  
      
 
  setSelectedOptions = (selectedOptions) => {
    
    this.setState({ selectedOptions });
  }
  getDropdownButtonLabel = ({ placeholderButtonLabel, value }) => {
    
    if(value === undefined){
      value = this.state.lstTaskOwnerName.map((item) => {
        return { value: item.UserProfileTableID }            
      });
      value.push({Label:"All",value:-1})
    }
      if (value?.some((o) => o.value === -1)) {
        return `${placeholderButtonLabel}: ALL`;
      } else {
        return `${placeholderButtonLabel}: ${value?.length} selected`;
     
    }
  }
  handleChange  = (value, event) => {
    
    if (event.action === "select-option" && event.option.value === -1) {
    this.setSelectedOptions(this.state.lstTaskOwnerName_List);
    }
    else if (
      event.action === "deselect-option" &&
      event.option.value === -1
    ) {
      this.setState({selectedOptions:[]});
    } 

    else if (event.action === "deselect-option") {
      // this.setState({selectedOptions:value.filter((o) => o.value !== -1)});
      var val = ["-1"];
      this.setState(
        {
          AdminReport: {
            ...this.state.AdminReport,
            TaskOwner: val,
          },
        },
        () => {
          this.setState({
            selectedOptions: value.filter((o) => o.value !== -1),
          });
        }
      );
    }
     else if (value.length === this.state.lstTaskOwnerName_List.length - 1) {
      this.setState({selectedOptions:value});
    } 
     else {
      this.setState({selectedOptions:value});

    }
    if(value.length > 0){
      this.ReportFiltercheckboxHandleChange(value);
    }
  }
 
  ReportFiltercheckboxHandleChange = (e) => {
    
    try {
      var arry=[];
      var target = e[e.length-1];
      var value = target.value;
      var name = "TaskOwner";

      for (let i = 0; i < e.length; i++) {
        var checkval = e[i].value === undefined ? e[i] : e[i]?.value;
        if (checkval === -1) {
          arry = [-1];
          break;
        } else {
          arry.push(e[i].value === undefined ? e[i] : e[i]?.value);
        }

        // arry.push(e[i].value);
      }
      this.setState(
        {
          AdminReport: {
            ...this.state.AdminReport,
            [name]: arry,
          },
        },

        () => {
          
          if (
            this.state.AdminReport.MainTask != null &&
            this.state.ProjectIdFilter !==""
          ) {
            this.LoadSubTasks(
              this.state.ProjectIdFilter,
              this.state.AdminReport.MainTask
            );
          }
          if (name=== "Department") {
            this.LoadTasksOwnerName();
            if (
              (this.state.ProjectIdFilter != null ||
                this.state.ProjectIdFilter !== "") &&
              value > 0
            ) {
              this.LoadMainTasksMappedByFilter();
            } else {
              this.LoadMainTasks();
            }
          }
       
        }

      );
    } catch (ex) {
      alert(ex);
    }
  };
  ReportFilterHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState(
        {
          AdminReport: {
            ...this.state.AdminReport,
            [name]: value,
          },
        },
        () => {
          if (
            this.state.AdminReport.MainTask != null &&
            this.state.ProjectIdFilter != ""
          ) {
            this.LoadSubTasks(
              this.state.ProjectIdFilter,
              this.state.AdminReport.MainTask
            );
          }
          if (name == "Department") {
            this.LoadTasksOwnerName();
            if (
              (this.state.ProjectIdFilter != null ||
                this.state.ProjectIdFilter != "") &&
              value > 0
            ) {
              this.LoadMainTasksMappedByFilter();
            } else {
              this.LoadMainTasks();
            }
          }
        }
      );
    } catch (ex) {
      alert(ex);
    }
  };
  SelectExportType = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        AdminReport: {
          ...this.state.AdminReport,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  UpdateActualDurationHandleChange = (event) => {
    try {
      const UpdateDuration = event.target.validity.valid
        ? event.target.value
        : "";
      this.setState({
        UpdateRecordFields: {
          ...this.state.UpdateRecordFields,
          ActualDuration: UpdateDuration,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  UpdateStateResource = (e) => {
    this.setState({
      UpdateRecordFields: {
        ...this.state.UpdateRecordFields,
        TaskOwnerID: e.TaskOwnerID,
        TaskOwnerName: e.TaskOwnerName,
      },
    });
    this.setState({ SaveAssignment: true });
  };
  UpdateRecordHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;

      this.setState(
        {
          UpdateRecordFields: {
            ...this.state.UpdateRecordFields,
            [name]: value,
          },
        },
        () => {
          if (name == "ProjectName") {
            this.LoadMainTasksMapped();
          }

          if (name == "MainTaskID") {
            if (
              this.state.UpdateRecordFields.ProjectID != null &&
              this.state.UpdateRecordFields.MainTaskID != null
            ) {
              this.LoadSubTasks(
                this.state.UpdateRecordFields.ProjectID,
                this.state.UpdateRecordFields.MainTaskID
              );
            }
          }
        }
      );
    } catch (ex) {
      //alert(ex);
    }
  };
  UpdateRecordHandleChangeDropdown = (e) => {
    try {
      this.LoadMainTasksMapped();
      if (
        this.state.UpdateRecordFields.ProjectID != null &&
        this.state.UpdateRecordFields.MainTaskID != null
      ) {
        this.LoadSubTasks(
          this.state.UpdateRecordFields.ProjectID,
          this.state.UpdateRecordFields.MainTaskID
        );
      }
      // this.setState(
      //   {
      //     UpdateRecordFields: {
      //       ...this.state.UpdateRecordFields,
      //       [name]: value,
      //     },
      //   },
      //   () => {
      //     if (name == "ProjectName") {

      //     }

      //     if (name == "MainTaskID") {
      //       if (
      //         this.state.UpdateRecordFields.ProjectID != null &&
      //         this.state.UpdateRecordFields.MainTaskID != null
      //       ) {
      //         this.LoadSubTasks(
      //           this.state.UpdateRecordFields.ProjectID,
      //           this.state.UpdateRecordFields.MainTaskID
      //         );
      //       }
      //     }
      //   }
      // );
    } catch (ex) {
      //alert(ex);
    }
  };
  UpdateRecordCheckboxHandle = () => {
    if (this.state.UpdateRecordFields.UpdateCheckBox == true) {
      this.setState({
        UpdateRecordFields: {
          ...this.state.UpdateRecordFields,
          UpdateCheckBox: false,
        },
      });
    } else {
      this.setState({
        UpdateRecordFields: {
          ...this.state.UpdateRecordFields,
          UpdateCheckBox: true,
        },
      });
    }
  };
  UpdateProjectDropDownSelectedValue = (e) => {
    try {
      this.setState(
        {
          UpdateRecordFields: {
            ...this.state.UpdateRecordFields,
            ProjectName: "",
            ProjectID: null,
          },
        },
        () => {
          this.LoadMainTasksMapped();
          if (
            this.state.UpdateRecordFields.ProjectID != null &&
            this.state.UpdateRecordFields.MainTaskID != null
          ) {
            this.LoadSubTasks(
              this.state.UpdateRecordFields.ProjectID,
              this.state.UpdateRecordFields.MainTaskID
            );
          }
        }
      );
    } catch (ex) {
      alert(ex);
    }
  };
  UpdateProjectDropDownSelectedValue = (e) => {
    try {
      this.setState(
        {
          UpdateRecordFields: {
            ...this.state.UpdateRecordFields,
            ProjectName: e.target.value,
            ProjectID:
              e.target[e.target.selectedIndex].getAttribute("projectid"),
          },
        },
        () => {
          this.LoadMainTasksMapped();
          if (
            this.state.UpdateRecordFields.ProjectID != null &&
            this.state.UpdateRecordFields.MainTaskID != null
          ) {
            this.LoadSubTasks(
              this.state.UpdateRecordFields.ProjectID,
              this.state.UpdateRecordFields.MainTaskID
            );
          }
        }
      );
    } catch (ex) {
      alert(ex);
    }
  };
  UpdateStartHandleDate = (date) => {
    try {
      this.setState({
        UpdateRecordFields: {
          ...this.state.UpdateRecordFields,
          ProjectDate: date,
          APIProjectDate: moment(date).format("MM/DD/YYYY"),
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  handleChangeRows = (page, rowpcount) => {
    this.setState({ rowsPerPage: parseInt(rowpcount, 10) });
    this.setState({ page: page });
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };

  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };

  OpenfilterBox = () => {
    this.setState({ PopUpBit: true });
  };

  GetCustomSearchValues = () => {
    debugger
    this.setState(
      {
        AdminReportBackup: {
          ...this.state.AdminReportBackup,
          ProjectType: this.state.AdminReport.ProjectType,
          MainTask: this.state.AdminReport.MainTask,
          SubTask: this.state.AdminReport.SubTask,
          TaskOwner: this.state.AdminReport.TaskOwner,
          ProjectId: this.state.ProjectIdFilter,
          ClientID: this.state.ClientIDSelected,
          StartDate: this.state.StartDate,
          EndDate: this.state.EndDate,
          ProjectIdFilterShow: this.state.ProjectIdFilterShow,
          ShowStartDate: this.state.ShowStartDate,
          ShowEndDate: this.state.ShowEndDate,
          IsFilter: true,
          ExportType: this.state.AdminReport.ExportType,
          Department: this.state.AdminReport.Department,
          ShowSubTask: this.state.AdminReport.ShowSubTask,
        },
      },
      () => {
        //this.setState({ PopUpBit: false });
      }
    );
  };

  handleClose = () => {
    this.setState(
      {
        AdminReport: {
          ...this.state.AdminReport,
          ProjectType: this.state.AdminReportBackup.ProjectType,
          MainTask: this.state.AdminReportBackup.MainTask,
          SubTask: this.state.AdminReportBackup.SubTask,
          TaskOwner: this.state.AdminReportBackup.TaskOwner,
          ExportType: this.state.AdminReportBackup.ExportType,
          Department: this.state.AdminReportBackup.Department,
          ShowSubTask: this.state.AdminReportBackup.ShowSubTask,
        },
        ClientIDSelected: this.state.AdminReportBackup.ClientID,
        StartDate: this.state.AdminReportBackup.StartDate,
        EndDate: this.state.AdminReportBackup.EndDate,
        ProjectIdFilter: this.state.AdminReportBackup.ProjectId,
        ProjectIdFilterShow: this.state.AdminReportBackup.ProjectIdFilterShow,
        ShowStartDate: this.state.AdminReportBackup.ShowStartDate,
        ShowEndDate: this.state.AdminReportBackup.ShowEndDate,
      },
      () => {
        this.setState({ PopUpBit: false });
        if (
          this.state.AdminReportBackup.ProjectId != null &&
          this.state.AdminReportBackup.MainTask != null
        ) {
          this.LoadSubTasks(
            this.state.AdminReportBackup.ProjectId,
            this.state.AdminReportBackup.MainTask
          );
        }
      }
    );
    // this.setState({ ProjectIdFilter: null });

    // this.setState({
    //   StartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
    //   EndDate: new Date(),
    //   ShowStartDate: new Date(Date.now() - 9 * 24 * 60 * 60 * 1000),
    //   ShowEndDate: new Date(),
    // });
  };

  refreshGrid = (e) => {
    debugger
    try {
      this.props.NotRefreshReportAction(); //use to set the redux RefreshBit to false
      this.setState({ Loading: true, ClientIDSelected: null });
      if (!this.state.IsUpdated) {
        this.setState({
          StartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        });
        this.setState({ EndDate: new Date() });
        this.setState({
          ShowStartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        });
        this.setState({ ShowEndDate: new Date() });
        this.setState({ page: 0 });
      }
      this.setState({ sortdatecheck: false });
      this.setState({ sortnamecheck: false });
      this.setState({ ProjectIdFilterShow: "" });
      this.setState({ HoursDays: "" });
      this.setState({ HoursDaysShow: "---Select---" });
      this.setState({ ProjectIdFilter: null });
      this.setState({ ShowZeroHours: null });
      this.setState({Differencehourse:null});
      this.setState({selectedOptions:this.state.lstTaskOwnerName_List});
      this.setState({ ExportIsCheck: null });
      this.setState({
        AdminReport: {
      
          TaskOwner: "",
          ProjectType: "",
          MainTask: null,
          SubTask: null,
          TaskOwner: null,
        },
      });

      this.setState({ ShowFilterTop: false });
      var LoggedInUserId =
        Cookies.get("Role") === "SuperAdmin" || Cookies.get("Role") === "Admin"
          ? this.state.AdminReport.TaskOwner === null
            ? -1
            : this.state.AdminReport.TaskOwner
          : Cookies.get("UserID");
      const objRptParam = {
        FromDate: this.state.StartDate,
        IsApproved: null,
        LoggedInUserId: LoggedInUserId,
        MainTaskID: -1,
        ProjectId: -1,
        SubTaskID: -1,
        ToDate: "",
        DepartmentID: -1,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/GetAssignmentsGrid`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: objRptParam,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ PopUpBit: false });
        var tempList = [];
        var _tempCount = 0;
        if (res.data.Result !== null) {
          res.data.Result.map((item) => {
            if (item.ProjectName === "TOTAL") {
              _tempCount = _tempCount + 0;
            } else {
              _tempCount++;
              tempList.push(item);
            }
          });
        }
        // for (var i = 0; i <= 2; i++) {
        //   tempList.sort(function (a, b) {
        //     a = a.AssignmentDateTime.split("/");
        //     /* a[0] = day
        //     a[1] = month
        //     a[2] = year */
        //     var _tempa = new Date(a[2], a[1], a[0]);
        //     b = b.AssignmentDateTime.split("/");
        //     var _tempb = new Date(b[2], b[1], b[0]);
        //     return _tempb - _tempa;
        //   });
        // }

        this.setState({ ShowFilterTop: true });
        this.setState({ lstReport: tempList });
        this.setState({ ReportCount: _tempCount });
        this.setState({ Loading: false });

        this.RefreshGridData();
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };

  RefreshGridData = () => {
    
    try {
      this.setState({ GrouplstReport: [] });
      this.setState({ ShowFilterTop: true });
      if (this.state.HoursDays == "Days") {
        this.setState({ HoursDaysShow: "Days" });
      } else if (this.state.HoursDays == "Hours") {
        this.setState({ HoursDaysShow: "Hours" });
      } else if (this.state.HoursDays == "Hours/Days") {
        this.setState({ HoursDaysShow: "Hours/Days" });
      } else {
        this.setState({ HoursDaysShow: "---Select---" });
      }
      var result = this.DateValidationchecks();
      if (!result) {
        return false;
      }
      this.setState({ PopUpBit: false });
      this.setState({ Loading: true }, () => {
        if (this.state.AdminReportBackup.IsFilter) {
          this.handleClose();
        }
      });
      var loggin=[];
      loggin= Cookies.get("Role") === "SuperAdmin" || Cookies.get("Role") === "Admin"
      ? this.state.AdminReport.TaskOwner == null
        ? -1
        : this.state.AdminReport.TaskOwner
      : [Cookies.get("UserID")];
      var LoggedInUserId =loggin;
      const objRptParam = {
        FromDate: this.state.StartDate,
        IsApproved: null,

        LoggedInUserId: LoggedInUserId,
        MainTaskID: this.state.AdminReport.MainTask,
        ProjectId:
          this.state.ProjectIdFilter == null || this.state.ProjectIdFilter == ""
            ? -1
            : this.state.ProjectIdFilter,
        SubTaskID: this.state.AdminReport.SubTask,
        ToDate: this.state.EndDate,
        ClientID: this.state.ClientIDSelected,
        DepartmentID:
          this.state.AdminReport.Department == null
            ? -1
            : this.state.AdminReport.Department == ""
            ? -1
            : this.state.AdminReport.Department,
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/GetAssignmentsGrid`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: objRptParam,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        var roletype = Cookies.get("Role");

        this.LoadProjectsHours();
        var tempList = [];
        var _tempCount = 0;
        if (res.data.Result !== null) {
          res.data.Result.map((item) => {
            if (item.ProjectName === "TOTAL") {
              _tempCount = _tempCount + 0;
            } else {
              _tempCount++;
              tempList.push(item);
            }
          });
        }
        if (roletype == "Staff") {
          var getlist = tempList.filter((Name) => Name.AssignmentID > 0);
          tempList = [];
          tempList = getlist;
          _tempCount = getlist.length;
        }
        // for (var i = 0; i <= 2; i++) {
        //   tempList.sort(function (a, b) {
        //     a = a.AssignmentDateTime.split("/");
        //     /* a[0] = day
        //     a[1] = month
        //     a[2] = year */
        //     var _tempa = new Date(a[2], a[1], a[0]);
        //     b = b.AssignmentDateTime.split("/");
        //     var _tempb = new Date(b[2], b[1], b[0]);
        //     return _tempb - _tempa;
        //   });
        // }
        this.setState({ lstReport: tempList });
        this.setState({ ReportCount: _tempCount });
        this.setState({ Loading: false });
        var grouparray = [];
        for (var i = 0; i < res.data.Result.length; i++) {
          if (grouparray.length == 0) {
            var obj = {
              ProjectNameAll: res.data.Result[i]?.ProjectName,
              ProjectIdAll: res.data.Result[i]?.ProjectID,
              ReferenceNumber: null,
              ClientID: res.data.Result[i]?.ClientID,
            };
            grouparray.push(obj);
          } else {
            var CheckProject = grouparray.filter(
              (SName) => SName.ProjectIdAll === res.data.Result[i]?.ProjectID
            );
            if (CheckProject.length == 0) {
              var obj = {
                ProjectNameAll: res.data.Result[i]?.ProjectName,
                ProjectIdAll: res.data.Result[i]?.ProjectID,
                ReferenceNumber: null,
                ClientID: res.data.Result[i]?.ClientID,
              };
              grouparray.push(obj);
            }
          }
        }
        if (grouparray.length > 0) {
          grouparray = grouparray.filter((Name) => Name.ProjectIdAll != null);
        }

        const result = grouparray.reduce((acc, d) => {
          const found = acc.find((a) => a.ClientID === d.ClientID);

          const clientname = this.state.lstClient.find(
            (a) => a.ID === d.ClientID
          );

          //const value = { name: d.name, val: d.value };
          const value = {
            ClientID: d.ClientID,
            value:
              clientname == undefined
                ? d.ProjectNameAll
                : clientname.ClientName,
            count: d.count,
          }; // the element in data property
          if (!found) {
            //acc.push(...value);
            acc.push({
              ClientID: d.ClientID > 0 ? d.ClientID : null,
              name:
                clientname == undefined
                  ? d.ProjectNameAll
                  : clientname.ClientName,
              data: [d],
            }); // not found, so need to add data property
          } else {
            //acc.push({ name: d.name, data: [{ value: d.value }, { count: d.count }] });
            found.data.push(value); // if found, that means data property exists, so just push new element to found.data.
          }
          return acc;
        }, []);

        this.setState({ GrouplstReport: grouparray, GroupByProject: result });
        this.setState({ GrouplstReport: grouparray });
        this.CustomColumnFilter();
      });

      // }
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  StartHandleDate = (date) => {
    try {
      this.setState({
        ShowStartDate: date,
      });
      var tempDate = moment(date).format("MM/DD/YYYY");
      this.setState({
        StartDate: tempDate,
      });
    } catch (ex) {
      alert(ex);
    }
  };

  EndHandleDate = (date) => {
    try {
      this.setState({
        ShowEndDate: date,
      });
      var tempDate = moment(date).format("MM/DD/YYYY");
      this.setState({
        EndDate: tempDate,
      });
    } catch (ex) {
      alert(ex);
    }
  };

  DateValidationchecks = () => {
    if (this.state.StartDate === "Invalid date") {
      this.setState({ DateErrMsg: "Please select date properly." });
      this.setState({ ErrorModel: true });
      return false;
    }
    if (this.state.EndDate === "Invalid date") {
      this.setState({ DateErrMsg: "Please select date properly." });
      this.setState({ ErrorModel: true });
      return false;
    } else {
      var _tempValue = moment(this.state.StartDate).diff(
        this.state.EndDate,
        "days"
      );
      if (_tempValue <= 0) {
        this.setState({ ErrorModel: false });
        return true;
      } else {
        this.setState({
          DateErrMsg: "Start date cannot be greater than end date.",
        });
        this.setState({ ErrorModel: true });
        return false;
      }
    }
  };

  HoursDaysDropDownSelectedValue = (e) => {
    try {
      this.setState({ HoursDays: e.target.value });
    } catch (ex) {
      alert(ex);
    }
  };
  ChangeClientID = (e) => {
    this.setState(
      {
        ClientIDSelected: e,
      },
      () => {
        if (this.state.ClientIDSelected != "") {
          var getproject = this.state.lstProjectNamesAll.filter(
            (SName) => SName.ClientID == this.state.ClientIDSelected
          );
          this.setState({
            lstProjectNames: getproject,
          });
        } else {
          this.setState({
            lstProjectNames: this.state.lstProjectNamesAll,
          });
        }
      }
    );
  };
  LoadMainTasksMappedByFilter = () => {
    try {
      const ProjectId = {
        ProjectID: this.state.ProjectIdFilter,
        UserID: this.state.AdminReport.Department,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTaskMappedListByDepartmentID`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectId,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMainTask: res.data.Result }, () => {
          this.LoadSubTasks(
            this.state.UpdateRecordFields.ProjectID,
            this.state.UpdateRecordFields.MainTaskID
          );
        });
        // this.setState({ lstMaintasksMapped: res.data.Result }, () => {
        //     //need to do this for update case...when we select project field maintask select should update

        //     // this.setState(
        //     //   {
        //     //     UpdateRecordFields: {
        //     //       ...this.state.UpdateRecordFields,
        //     //       Maintask: this.state.lstMaintasksMapped[0]?.MainTaskName,
        //     //       MainTaskID: this.state.lstMaintasksMapped[0]?.Id,
        //     //     },
        //     //   },
        //     //   () => {
        //     //     this.LoadSubTasks(
        //     //       this.state.UpdateRecordFields.ProjectID,
        //     //       this.state.UpdateRecordFields.MainTaskID
        //     //     );
        //     //   }
        //     // );

        // });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ProjectDropDownSelectedValue = (e) => {
    try {
      this.setState({
        ProjectIdFilter: e.target.value,
      });
      this.setState(
        {
          ProjectIdFilterShow:
            e.target[e.target.selectedIndex].getAttribute("name"),
        },
        () => {
          this.setState(
            {
              AdminReport: {
                ...this.state.AdminReport,
                MainTask: "",
                ExportType: this.state.ProjectIdFilter == "" ? true : false,
              },
            },
            () => {
              if (
                this.state.ProjectIdFilter == "" &&
                (this.state.AdminReport.Department == "" ||
                  this.state.AdminReport.Department == null)
              ) {
                this.LoadMainTasks();
                this.LoadSubTasks(
                  this.state.UpdateRecordFields.ProjectID,
                  this.state.UpdateRecordFields.MainTaskID
                );
              } else {
                this.LoadMainTasksMappedByFilter();
              }
            }
          );
        }
      );
    } catch (ex) {
      alert(ex);
    }
  };

  CloseupdateModal = () => {
    this.setState({
      ConfrimMsgPopUp: false,
      IsUpdated: false,
      RecordUpdateStatus: false,
    });
  };

  CheckConfirm = () => {
    //this.CloseModal();\

    if (this.state.RecordUpdateStatus == true) {
      this.UpdateRecords();
    } else {
      this.setState({ ShowMsgPopUp: false });
      this.setState({ ProjectDescription: "" });
      this.setState({ ProjectName: "" });
      this.setState({ TaskOwnerID: null });
      this.setState({ TaskName: "" });
      this.setState({ Comment: "" });
      this.setState({ MainTaskID: "" });
      this.setState({ ProjectId: null });
      this.setState({ PhaseDescription: null });
      this.setState({ ActualDuration: "" });
      this.setState({ errorStyle: {} });

      this.CloseModal();
    }
    this.setState({ ConfrimMsgPopUp: false });
  };

  ConfrimModals = () => {
    this.setState({ Title: "Update  Record" });
    this.setState({ errorMsg: "Are you sure you want to exit ? " });
    this.setState({ ConfrimMsgPopUp: true });
  };

  CloseModal = () => {
    this.setState({ ErrorModel: false });
    this.setState({ showUpdateModel: false }, () => {
      this.setState({
        UpdateRecordFields: {
          AssignmentID: null,
          ProjectID: null,
          ProjectName: "",
          TaskOwnerName: "",
          TaskOwnerID: null,
          ProjectDescription: "",
          ActualDuration: null,
          Maintask: "",
          Subtask: "",
          SubtaskID: "",
          Comment: "",
          ProjectDate: false,
          APIProjectDate: "",
          MainTaskID: null,
          SaveAssignment: true,
        },
        RecordUpdateStatus: false,
      });
      // this.LoadMainTasks();
      this.setState({ lstSubtask: null });

      // if (!this.state.IsUpdated) {
      //   this.setState({
      //     StartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
      //     EndDate: new Date(),
      //     ShowStartDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
      //     ShowEndDate: new Date(),
      //   });
      // }
      this.GetCustomSearchValues();
    });
  };
  UpdateStateProject = (e) => {
    // this.setState(
    //   {
    //     UpdateRecordFields: {
    //       ...this.state.UpdateRecordFields,
    //       ProjectID: e.ProjectID,
    //       ProjectName: e.ProjectName,
    //     },
    //   },
    //   () => {
    //     this.LoadMainTasks();
    //     this.setState({ lstSubtask: null, SaveAssignment: false });
    //   }
    // );
    try {
      this.setState(
        {
          UpdateRecordFields: {
            ...this.state.UpdateRecordFields,
            ProjectID: e.ProjectID,
            ProjectName: e.ProjectName,
          },
        },
        () => {
          this.LoadMainTasksMapped();
          if (
            this.state.UpdateRecordFields.ProjectID != null &&
            this.state.UpdateRecordFields.MainTaskID != null
          ) {
            this.LoadSubTasks(
              this.state.UpdateRecordFields.ProjectID,
              this.state.UpdateRecordFields.MainTaskID
            );
          }
        }
      );
    } catch (ex) {
      alert(ex);
    }
  };
  EmptyProjectDropdown = () => {
    this.setState(
      {
        UpdateRecordFields: {
          ...this.state.UpdateRecordFields,
          ProjectID: null,
          ProjectName: "",
        },
      },
      () => {
        this.LoadMainTasks();
        this.setState({ lstSubtask: null, SaveAssignment: false });
      }
    );
  };
  TimeSheetValidationCheck = () => {
    this.setState({ Loading: true });
    if (
      this.state.ProjectIdFilter == null ||
      this.state.ProjectIdFilter == ""
    ) {
      this.setState({
        ErrorModel: true,
        DateErrMsg: "Please select Project",
      });
      this.setState({ Loading: false });
      return false;
    } else if (this.state.StartDate == "" || this.state.EndDate == "") {
      this.setState({
        ErrorModel: true,
        DateErrMsg: "Please select Date",
      });
      this.setState({ Loading: false });
      return false;
    } else {
      return true;
    }
  };
  // sortBydefault(dataArray) {
  //   //changing date format for applying the sorting
  //   var date = dataArray.forEach((item, index) => {
  //     var date = item.AssignmentDateTime.split("/");
  //     var year = date[2];
  //     var month = date[1];
  //     var day = date[0];

  //     var newDate = year + "-" + month + "-" + day;
  //     item.AssignmentDateTime = newDate;
  //   });

  //   dataArray.sort(function (a, b) {
  //     var dateA = new Date(a.AssignmentDateTime),
  //       dateB = new Date(b.AssignmentDateTime);
  //     return dateB - dateA;
  //   });

  //   dataArray.forEach((item, index) => {
  //     var date = item.AssignmentDateTime.split("-");
  //     var year = date[0];
  //     var month = date[1];
  //     var day = date[2];

  //     var newDate = day + "/" + month + "/" + year;

  //     item.AssignmentDateTime = newDate;
  //     this.setState(date);
  //   });
  //   return dataArray;
  // }

  //ascending sort data grid by project names
  // sortByProjectName(dataArray, key) {
  //   this.setState({ Loading: true });
  //   var data = dataArray.sort(function (a, b) {
  //     var x = a[key];
  //     var y = b[key];
  //     return x < y ? -1 : x > y ? 1 : 0;
  //   });
  //   this.setState({ Loading: false });
  //   this.setState(data);
  // }

  //ascending sort data grid by date
  // sortByDate(dataArray) {
  //   //changing date format for applying the sorting
  //   var date = dataArray.forEach((item, index) => {
  //     var date = item.AssignmentDateTime.split("/");
  //     var year = date[2];
  //     var month = date[1];
  //     var day = date[0];

  //     var newDate = year + "-" + month + "-" + day;
  //     item.AssignmentDateTime = newDate;
  //   });

  //   dataArray.sort(function (a, b) {
  //     var dateA = new Date(a.AssignmentDateTime),
  //       dateB = new Date(b.AssignmentDateTime);
  //     return dateA - dateB;
  //   });

  //   // dataArray.forEach((item, index) => {
  //   //   var date = item.AssignmentDateTime.split("-");
  //   //   var year = date[0];
  //   //   var month = date[1];
  //   //   var day = date[2];

  //   //   var newDate = day + "/" + month + "/" + year;

  //   //   item.AssignmentDateTime = newDate;
  //   //   this.setState(date);
  //   // });
  //   return dataArray;
  // }
  BreakDown = () => {
    try {
      this.setState({ Loading: true });
      var FileName =
        "_" +
        moment(this.state.StartDate).format("DD_MM_YYYY") +
        "-" +
        moment(this.state.EndDate).format("DD_MM_YYYY");
      const ExportAssignment = {
        ProjectId: this.state.ProjectIdFilter,
        TaskOwnerId:
          this.state.AdminReport.TaskOwner != null
            ? this.state.AdminReport.TaskOwner
            : "All",
        FromDate: this.state.StartDate,
        ToDate: this.state.EndDate,
        TaskId:
          this.state.AdminReport.MainTask == null
            ? -1
            : this.state.AdminReport.MainTask == ""
            ? -1
            : this.state.AdminReport.MainTask,
        SubTaskId:
          this.state.AdminReport.SubTask == null
            ? -1
            : this.state.AdminReport.SubTask == ""
            ? -1
            : this.state.AdminReport.SubTask,
        Billable: this.state.ExportIsCheck == true ? 1 : 0,
        IsZeroRowShow: this.state.ShowZeroHours == true ? 1 : 0,
        IsDifferencehourse:this.state.Differencehourse == true ? 1 : 0,
        SettingDateRange: 0,
        fileName: FileName,
        AllArrays: this.state.GrouplstReport,
        ClientArray: this.state.lstClient,
        ExportType: this.state.AdminReport.ExportType,
        ClientArray: this.state.lstClient,
        DepartmentID:
          this.state.AdminReport.Department == null
            ? -1
            : this.state.AdminReport.Department == ""
            ? -1
            : this.state.AdminReport.Department,
        ShowSubTask: this.state.AdminReport.ShowSubTask,
      };

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/ExportToPdfMainTaskBreakDown`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ExportAssignment,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 409) {
          //409 = conflict on server
          this.setState({
            ErrorModel: true,
            DateErrMsg: res.data.Result,
          });
        } else if (res.data.Result == "Failed") {
          this.setState({
            ErrorModel: true,
            DateErrMsg: "No Record Avaliable",
          });
        } else {
          for (var i = 0; i < res.data.Result.length; i++) {
            window.open(
              `http://localhost:1316/PdfFiles/${res.data.Result[i]}`,
              "_blank"
            );
          }
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  WBSExcelExport = () => {
    try {
      this.setState({ Loading: true });

      axios({
        method: "Post",
        url:
          `${process.env.REACT_APP_BASE_URL}Reports/WBSExcelExport?ProjectID=` +
          this.state.WBSExcelProjectID +
          `&ProjectName=` +
          this.state.WBSExcelProjectName,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 409) {
          //409 = conflict on server
          this.setState({
            ErrorModel: true,
            DateErrMsg: res.data.Result,
          });
        } else if (res.data.Result == "Failed") {
          this.setState({
            ErrorModel: true,
            DateErrMsg: "No Record Avaliable",
          });
        } else {
          window.open(
            `http://localhost:1316/ExcelFiles/${res.data.Result}`,
            "_blank"
          );
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ColumnsHideSHow = (index, Status) => {
    var status = Status ? false : true;
    let markers = [...this.state.ReportColumnName];
    markers[index] = {
      ...markers[index],
      Status: status,
    };
    this.setState({ ReportColumnName: markers });
  };
  ColumnsFilter = (index, Status) => {
    var status = Status;
    let markers = [...this.state.ReportColumnName];
    markers[index] = {
      ...markers[index],
      Status: status,
    };
    this.setState({ ReportColumnName: markers });
  };

  LoadProjectsHours = () => {
    try {
      const ProjectcObj = {
        ProjectId: this.state.ProjectIdFilter,
        TaskName: "",
        TaskOwnerId: this.state.AdminReport.TaskOwner,
        FromDate: this.state.StartDate,
        ToDate: this.state.EndDate,
        IsApproved: "",
        MainTaskID: this.state.AdminReport.MainTask,
        SubTaskID: this.state.AdminReport.SubTask,
        ClientID: this.state.ClientIDSelected,
        DepartmentID:
          this.state.AdminReport.Department == null
            ? -1
            : this.state.AdminReport.Department == ""
            ? -1
            : this.state.AdminReport.Department,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/GetTotalHoursForTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectcObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ ProjectHoursList: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  CustomColumnFilter = () => {
    
    let markers = [...this.state.ReportColumnName];
    for (var i = 6; i < 9; i++) {
      if (this.state.HoursDays == "Hours") {
        var status = this.state.HoursDays == "Hours" && i != 6 ? true : false;
        markers[i] = {
          ...markers[i],
          Status: status,
        };
      }
      if (this.state.HoursDays == "Days") {
        var status = this.state.HoursDays == "Days" && i != 6 ? false : true;
        markers[i] = {
          ...markers[i],
          Status: status,
        };
      }
      if (
        this.state.HoursDays == "---Select---" ||
        this.state.HoursDays == ""
      ) {
        markers[i] = {
          ...markers[i],
          Status: true,
        };
      }
      //this.ColumnsFilter(i, status);
    }
    this.setState({ ReportColumnName: markers });
  };
  getrowdata = (row) => {
    
    this.setState(
      {
        UpdateRecordFields: {
          ...this.state.UpdateRecordFields,
          AssignmentID: row.AssignmentID,
          ProjectName: row.ProjectName,
          ProjectID: row.ProjectID,
          TaskOwnerName: row.UserFullName,
          TaskOwnerID: row.UserID,
          ActualDuration: row.ActualDuration,
          Maintask: row.MainTaskName,
          Subtask: row.TaskName,
          Comment: row.CommentText,
          ProjectDate: moment(row.AssignmentDateTime, "DD-MM-YYYY").toDate(),
          APIProjectDate: moment(row.AssignmentDateTime, "DD-MM-YYYY"),
          MainTaskID: row.MainTaskID,
          ProjectDescription: row.ProjectDescription,
          SubtaskID: row.TaskID,
          BillableHours: row.BillableHours,
          BillableIsApproved: row.IsBillableApproved,
        },
      },
      () => {
        this.LoadFirstMainTasksMapped();
        this.LoadFirstSubTasks(
          this.state.UpdateRecordFields.ProjectID,
          this.state.UpdateRecordFields.MainTaskID
        );
        if (this.state.lstProjectNames != null) {
          this.state.lstProjectNames.map((item) => {
            if (row.Project === item.Name) {
              this.setState({
                UpdateRecordFields: {
                  ...this.state.UpdateRecordFields,
                  ProjectDescription: item.ProjectDescription,
                },
              });
            }
          });
        }
        this.setState({
          showUpdateModel: true,
        });
      }
    );
  };

  //#endregion

  render() {
    var Report_ColumnName = null;
    var TaskOwnerNameList =null;
  TaskOwnerNameList=this.state.lstTaskOwnerName!=null? this.state.lstTaskOwnerName?.map((item) => {
        return { value: item.UserProfileTableID, label: item.Name }            
      }):null;
    // ReportColumnName =
    //   this.state.ReportColumnName !== null
    //     ? this.state.ReportColumnName.map((item, key) => {
    //         return (
    //           <>
    //             <Checkbox
    //               onClick={this.ColumnsHideSHow()}
    //               ddAttr={item.StateName}
    //               checked={item.Status}
    //             />
    //             <option
    //               //onClick={this._onSelect}
    //               ddAttr={item.StateName}
    //               value={item.StateName}
    //             >
    //               {item.ColumnName}
    //             </option>
    //           </>
    //         );
    //       })
    //     : "";
    Report_ColumnName =
      this.state.ReportColumnName !== null
        ? this.state.ReportColumnName.map((item, index) => {
            return (
              <li value={item.StateName}>
                <Checkbox
                  ddAttr={item.StateName}
                  onClick={() => {
                    this.ColumnsHideSHow(index, item.Status);
                  }}
                  checked={item.Status}
                />
                {item.ColumnName}
              </li>
            );
          })
        : "";
    var TotalHours =
      this.state.ProjectHoursList == null ||
      this.state.ProjectHoursList.length === 0 ? (
        <div className="col-md-4">
          {" "}
          <span>
            <strong> No Record Available</strong>
          </span>{" "}
        </div>
      ) : (
        this.state.ProjectHoursList.map((item) => {
          return (
            <div className="col-md-4">
              <strong>
                {item.ProjectName}({" "}
                <span style={{ fontWeight: "bold" }}> A: </span>
                <span style={{ fontWeight: "600" }}>
                  {item.TotalActualDuration} hrs
                </span>
                <span style={{ fontWeight: "bold" }}> B: </span>
                <span style={{ fontWeight: "600" }}>
                  {item.BillableHours} hrs
                </span>
                )
              </strong>{" "}
            </div>
          );
        })
      );
    var TotalDays =
      this.state.ProjectHoursList == null ||
      this.state.ProjectHoursList.length === 0 ? (
        <div className="col-md-4">
          {" "}
          <span>
            <strong> No Record Available</strong>
          </span>{" "}
        </div>
      ) : (
        this.state.ProjectHoursList.map((item) => {
          return (
            <div className="col-md-4">
              <strong>{item.ProjectName}</strong>{" "}
              <span>
                ( <strong> Days: </strong> {item.ActualD}
              </span>
              )
            </div>
          );
        })
      );
    return (
      <div ref="component">
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div
                className="row rpt_mbl_header"
                id="desk_view_reports"
                style={{ marginBottom: "20px" }}
              >
                <div className="col-sm-4 text-left mt-3">
                  <h3
                    className="m_font_heading"
                    style={{ marginBottom: "10px" }}
                  >
                    Reports
                  </h3>
                </div>
                <div className="col-sm-2 text-right ems_show_counter"
             >
                {/* <ButtonGroup size="sm">
                    <Row>
              {Cookies.get("Role") !== "Staff" ? (
                
                  <Tooltip
                    className="ex_combine"
                    title="Show SubTask in Export File"
                    arrow
                  >
                    <label>
                      <Checkbox
                        onClick={() => {
                          if (this.state.AdminReport.ShowSubTask === true) {
                            this.setState({
                              AdminReport: {
                                ...this.state.AdminReport,
                                ShowSubTask: false,
                              },
                            });
                          } else {
                            this.setState({
                              AdminReport: {
                                ...this.state.AdminReport,
                                ShowSubTask: true,
                              },
                            });
                          }
                        }}
                        checked={this.state.AdminReport.ShowSubTask}
                      />
                       Show Subtask
                    </label>
                  </Tooltip>
              ):("")}
              </Row>
              </ButtonGroup> */}
  
                </div>
                <div
                  className="col-sm-6 text-right ems_show_counter"
                  id="ems_hours_count"
                >
                  {/* <button type="button" className="btn-black mr-2">Export <i className="fa fa-plus"></i></button> */}

                  <ButtonGroup size="sm">
                    <Row>
                      {Cookies.get("Role") !== "Staff" ? (
                        <>
                          {/* <Tooltip
                            className="ex_combine"
                            title="Combine projects for a Client"
                            arrow
                          >
                            <label>
                              <Checkbox
                                onClick={() => {
                                  if (
                                    this.state.AdminReport.ExportType === true
                                  ) {
                                    this.setState({
                                      AdminReport: {
                                        ...this.state.AdminReport,
                                        ExportType: false,
                                      },
                                    });
                                  } else {
                                    this.setState({
                                      AdminReport: {
                                        ...this.state.AdminReport,
                                        ExportType: true,
                                      },
                                    });
                                  }
                                }}
                                checked={this.state.AdminReport.ExportType}
                              />
                              Export by Client
                            </label>
                          </Tooltip> */}

                          {/* <Tooltip
                            className="check_boox_h"
                            title="Show Zero Hours"
                            arrow
                          >
                            <label>
                              <Checkbox
                                onClick={() => {
                                  if (this.state.ShowZeroHours === true) {
                                    this.setState({ ShowZeroHours: false });
                                  } else {
                                    this.setState({ ShowZeroHours: true });
                                  }
                                }}
                                checked={this.state.ShowZeroHours}
                              />
                              Show 0 Hours
                            </label>
                          </Tooltip> */}
                        </>
                      ) : (
                        ""
                      )}
                    </Row>
                    {Cookies.get("Role") !== "Staff" ? (
                      <Dropdown
                        className="ems_data_exported ems_reported_options"
                        style={{ maxWidth: "200px" }}
                        menuAlign="left"
                       
                      >
                        <Dropdown.Toggle
                          variant="default report_btn_export ml-2"
                          id="dropdown-basic"
                          className="ems_export_toggle"
                          style={{
                            width: "10rem",
                            backgroundColor: "black",
                            color: "#fff",
                          }}
                        >
                          <span className="text-white">Report Options</span>
                        </Dropdown.Toggle>
                        <Dropdown.Menu className="align_top">
                        
                        <Button>   
                        <Checkbox
                                onClick={() => {
                                  if (
                                    this.state.AdminReport.ExportType === true
                                  ) {
                                    this.setState({
                                      AdminReport: {
                                        ...this.state.AdminReport,
                                        ExportType: false,
                                      },
                                    });
                                  } else {
                                    this.setState({
                                      AdminReport: {
                                        ...this.state.AdminReport,
                                        ExportType: true,
                                      },
                                    });
                                  }
                                }}
                                checked={this.state.AdminReport.ExportType}
                              />  Export by Client       
                        </Button>
                        <Button>   
                        <Checkbox
                                onClick={() => {
                                  
                                  if (this.state.ShowZeroHours === true) {
                                    this.setState({ ShowZeroHours: false });
                                  } else {
                                    this.setState({ ShowZeroHours: true });
                                    if (this.state.ShowZeroHours === false) {
                                      this.setState({ Differencehourse: false });
                                    }
                                  }
                                }}
                                checked={this.state.ShowZeroHours}
                              /> Show 0 Hours    
                        </Button>

                          <Button>   
                          <Checkbox
                          onClick={() => {
                            if (this.state.AdminReport.ShowSubTask === true) {
                            this.setState({
                              AdminReport: {
                                ...this.state.AdminReport,
                                ShowSubTask: false,
                              },
                            });
                          } else {
                            this.setState({
                              AdminReport: {
                                ...this.state.AdminReport,
                                ShowSubTask: true,
                              },
                            });
                          }
                        }}
                        checked={this.state.AdminReport.ShowSubTask}
                      />
                       Show Subtask
                        </Button>
                        <Button>   
                        <Checkbox
                          onClick={() => {
                            if (this.state.Differencehourse === true) {
                              this.setState({ Differencehourse: false });
                            } else {
                              this.setState({ Differencehourse: true });
                              if (this.state.Differencehourse === false){
                                this.setState({ ShowZeroHours: false });
                              }
                            }
                          }}
                          checked={this.state.Differencehourse}
                        /> Actual/Billable Different  
                        </Button>
                        </Dropdown.Menu>

                      </Dropdown>
                    ) : null}

                    {Cookies.get("Role") !== "Staff" ? (
                      <Dropdown
                        className="ems_data_exported"
                        style={{ maxWidth: "200px" }}
                        menuAlign="left"
                        onSelect={(evt) => {
                          // if (this.TimeSheetValidationCheck()) {
                          if (evt === "ExcelActualHrs") {
                            this.ExportExcelReportActualHrs();
                          } else if (evt == "Excel") {
                            this.ExportExcelReport();
                          } else if (evt == "WBSExcel") {
                            this.setState({ WBSExcelShow: true });
                          } else if (evt == "Pdf") {
                            if (Cookies.get("Role") === "SuperAdmin") {
                              this.state.GrouplstReport.map((a) => {
                                a.ReferenceNumber = null;
                              });
                              this.setState({
                                ExportToPdfModalShow: true,
                              });
                            } else {
                              this.ExportPdfReport();
                            }

                            // this.ExportPdfReport();
                          } else if (evt == "BreakDown") {
                            this.BreakDown();
                          }
                          // }
                        }}
                      >
                        <Dropdown.Toggle
                          variant="default report_btn_export ml-2"
                          id="dropdown-basic"
                          className="ems_export_toggle"
                          style={{
                            backgroundColor: "black",
                            color: "#fff",
                          }}
                        >
                          <Tooltip title="Check for Billable" arrow>
                            <Checkbox
                              onClick={() => {
                                if (this.state.ExportIsCheck == true) {
                                  this.setState({ ExportIsCheck: false });
                                } else {
                                  this.setState({ ExportIsCheck: true });
                                }
                              }}
                              checked={this.state.ExportIsCheck}
                            />
                          </Tooltip>
                          <span className="text-white">Export</span>
                        </Dropdown.Toggle>

                        <Dropdown.Menu className="align_top">
                          {this.state.ExportIsCheck == true ? (
                            <>
                              <Dropdown.Item eventKey="Excel">
                                Excel
                              </Dropdown.Item>
                              <Dropdown.Item eventKey="Pdf"> Pdf</Dropdown.Item>
                              <Dropdown.Item eventKey="BreakDown">
                                Main Task Breakdown
                              </Dropdown.Item>
                            </>
                          ) : (
                            <>
                              <Dropdown.Item eventKey="ExcelActualHrs">
                                Excel
                              </Dropdown.Item>
                              {Cookies.get("Role") === "SuperAdmin" ? (
                                <Dropdown.Item eventKey="WBSExcel">
                                  WBS Excel
                                </Dropdown.Item>
                              ) : null}
                            </>
                          )}
                        </Dropdown.Menu>
                      </Dropdown>
                    ) : null}
                    {/* <button
                      className="btn-black ems_filter_added ml-2"
                      onClick={() => {
                        this.setState({
                          ExportShow: true,
                        });
                      }}
                      type="button"
                      data-toggle="modal"
                      data-target="#reportsFilter"
                    >
                      Export
                    </button> */}
                    <button
                      className="btn-black ems_filter_added ml-2"
                      onClick={this.OpenfilterBox}
                      type="button"
                      data-toggle="modal"
                      data-target="#reportsFilter"
                    >
                      Filter <i className="menu-icon fa fa-plus"></i>
                    </button>

                    <button
                      className="btn-black ems_clear_filter ml-2"
                      onClick={() => {
                        this.setState(
                          {
                            StartDate: new Date(
                              Date.now() - 10 * 24 * 60 * 60 * 1000
                            ),
                            lstProjectNames: this.state.lstProjectNamesAll,
                            IsUpdated: false,
                            AdminReportBackup: {
                              ...this.state.AdminReportBackup,
                              IsFilter: false,
                            },
                          },

                          () => {
                            this.refreshGrid();
                          }
                        );
                      }}
                      type="button"
                      data-toggle="modal"
                      data-target="#reportsFilter"
                    >
                      Clear Filters
                    </button>
                  </ButtonGroup>
                  {/* {Cookies.get("Role") != "Staff" ? (
                    <div
                      className="btn_black_checkbox"
                      id="ems_data_sort_users"
                    >
                      <Dropdown>
                        <Dropdown.Toggle
                          variant="default report_btn_export"
                          id="dropdown-basic"
                          style={{
                            backgroundColor: "black",
                            color: "#fff",
                          }}
                        >
                          {this.state.sortnamecheck === true ? (
                            <Dropdown.Item eventKey="SortName">
                              Name
                            </Dropdown.Item>
                          ) : this.state.sortdatecheck === true ? (
                            <Dropdown.Item eventKey="SortDate">
                              Date
                            </Dropdown.Item>
                          ) : (
                            <option>Sort By</option>
                          )}
                        </Dropdown.Toggle>

                        <Dropdown.Menu className="align_top">
                          <div className="sorting_data">
                            <Tooltip title="Sort by Name" arrow>
                              <Checkbox
                                id="ems_custom_val"
                                onClick={() => {
                                  if (this.state.sortnamecheck == true) {
                                    this.sortBydefault(this.state.lstReport);
                                    this.setState({ sortnamecheck: false });
                                  } else {
                                    this.sortByProjectName(
                                      this.state.lstReport,
                                      "ProjectName"
                                    );
                                    this.setState({ sortnamecheck: true });
                                    this.setState({ sortdatecheck: false });
                                  }
                                }}
                                checked={this.state.sortnamecheck}
                              />
                            </Tooltip>
                            <Dropdown.Item eventKey="SortName">
                              Name
                            </Dropdown.Item>
                          </div>

                          <div className="secondary_data_sort">
                            <Tooltip title="Sort by Date" arrow>
                              <Checkbox
                                onClick={() => {
                                  if (this.state.sortdatecheck == true) {
                                    this.sortBydefault(this.state.lstReport);
                                    this.setState({ sortdatecheck: false });
                                  } else {
                                    this.sortByDate(this.state.lstReport);
                                    this.setState({ sortdatecheck: true });
                                    this.setState({ sortnamecheck: false });
                                  }
                                }}
                                checked={this.state.sortdatecheck}
                              />
                            </Tooltip>
                            <Dropdown.Item eventKey="SortDate">
                              Date
                            </Dropdown.Item>
                          </div>
                        </Dropdown.Menu>
                      </Dropdown>
                    </div>
                  ) : null} */}
                  {/* <select name="select" id="select" className="form-control">
                          <option value="excel">Export to Excel</option>
                          <option value="pdf">Export to PDF</option>
                      </select> */}
                </div>
              </div>

              {Cookies.get("Role") != "Staff" ? (
                <div className="card">
                  <div
                    className="card-header"
                    style={{ backgroundColor: "#f1f1f1" }}
                  >
                    <div
                      className="row"
                      style={{
                        fontSize: "12px",
                        fontFamily:
                          "Helvetica Neue,Helvetica,Arial,sans-serif;",
                      }}
                    >
                      {this.state.HoursDaysShow == "Hours" ||
                      this.state.HoursDaysShow == "---Select---"
                        ? TotalHours
                        : TotalDays}
                    </div>
                  </div>
                </div>
              ) : null}

              <div className="card">
                <div
                  className="card-header"
                  style={{ backgroundColor: "#f1f1f1" }}
                >
                  {Cookies.get("Role") === "SuperAdmin" ||
                  Cookies.get("Role") === "Admin" ? (
                    <span
                      className="ems_right_position pull-right"
                      onMouseLeave={() => {
                        this.setState({
                          ShowColumnChooser: false,
                        });
                      }}
                    >
                      <button
                        type="button"
                        className="btn-black ems_cl_filters"
                        onClick={() => {
                          if (this.state.ShowColumnChooser) {
                            this.setState({
                              ShowColumnChooser: false,
                            });
                          }
                          if (!this.state.ShowColumnChooser) {
                            this.setState({
                              ShowColumnChooser: true,
                            });
                          }
                        }}
                      >
                        Select Columns
                      </button>
                      <div className="ems_select_columns" id="ems_hours_count">
                        {this.state.ShowColumnChooser ? (
                          this.state.ReportColumnName != null ? (
                            this.state.ReportColumnName.length > 0 ? (
                              <div className="input_box">
                                <ul>{Report_ColumnName}</ul>
                              </div>
                            ) : null
                          ) : null
                        ) : null}
                      </div>
                    </span>
                  ) : null}
                  <h6 className="pt-2 pb-2 search_res" id="rep_grids">
                    Search Results : {this.state.ReportCount}
                  </h6>

                  <div className="row no-gutters">
                    <div className="col-sm-3" id="ems_start_point_date">
                      <span>
                        {this.state.ShowStartDate !== false ? (
                          <h6
                            className="search_res"
                            style={{
                              display: "flex",
                            }}
                          >
                            StartDate:{" "}
                            {moment(this.state.ShowStartDate).format(
                              "DD/MM/YYYY"
                            )}
                          </h6>
                        ) : null}
                      </span>
                    </div>

                    <div className="col-sm-3" id="ems_ending_point_date">
                      <span className="ems_ending_point">
                        {this.state.ShowEndDate != false ? (
                          <h6>
                            <span className="end_date_repo text-dark mr-3">
                              EndDate:
                            </span>
                            {moment(this.state.ShowEndDate).format(
                              "DD/MM/YYYY"
                            )}
                          </h6>
                        ) : null}
                      </span>
                    </div>
                    <div className="col-sm-6"></div>
                  </div>

                  {this.state.ShowFilterTop == true ? (
                    <div className="row">
                      {/* <div className="col-sm-3"></div> */}
                      {this.state.ProjectIdFilterShow !== "" &&
                      this.state.ProjectIdFilterShow !== null ? (
                        <div className="col-sm-3 pt-2">
                          <h6
                            style={{
                              display: "flex",
                            }}
                          >
                            ProjectName:
                            <p
                              style={{
                                marginLeft: "10px",
                                marginBottom: "0",
                                whiteSpace: "nowrap",
                                overflow: "hidden",
                                textOverflow: "ellipsis",
                              }}
                            >
                              {this.state.ProjectIdFilterShow}
                            </p>
                          </h6>
                        </div>
                      ) : null}

                      {this.state.HoursDaysShow != "---Select---" ? (
                        <div className="col-sm-3">
                          <h6 style={{ display: "flex" }}>
                            Hours or Days:{" "}
                            <p
                              style={{
                                marginLeft: "10px",
                                marginBottom: "0",
                                whiteSpace: "nowrap",
                                overflow: "hidden",
                                textOverflow: "ellipsis",
                              }}
                            >
                              {this.state.HoursDaysShow}
                            </p>{" "}
                          </h6>
                        </div>
                      ) : null}
                    </div>
                  ) : null}
                </div>

                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div
                    className="card-body ems_data_reports"
                    style={{ paddingTop: "0px" }}
                  >
                    {this.state.ReportColumnName.length > 0 &&
                    this.state.lstReport?.length > 0 ? (
                      <CustomColumn
                        TableHead={this.state.ReportColumnName}
                        lstReport={this.state.lstReport}
                        page={this.state.page}
                        rowsPerPage={this.state.rowsPerPage}
                        ReportCount={this.state.ReportCount}
                        GetData={(row) => {
                          this.getrowdata(row);
                        }}
                        checkadmin={Cookies.get("Role")}
                        ModuleName={"Report"}
                        IsUpdated={this.state.IsUpdated}
                        handleChangeRows={(page, rowcount) => {
                          this.handleChangeRows(page, rowcount);
                        }}
                      />
                    ) : null}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>

        <ReportFilter
          Title={this.state.FTitle}
          show={this.state.PopUpBit}
          onHide={this.handleClose}
          project={this.state.lstProjectNames}
          maintasks={this.state.lstMainTask}
          AdminHandleChange={this.ReportFilterHandleChange}
          AdminHandleChangecheckbox={this.ReportFiltercheckboxHandleChange}
          onchangehandle={this.handleChange}
          getDropdownButtonLabel={this.getDropdownButtonLabel}
          lstTaskOwnerName_List={this.state.lstTaskOwnerName_List}
          setSelectedOptions={this.state.selectedOptions}
          Selectedtaskowner={this.state.AdminReport}
          taskowernames={this.state.lstTaskOwnerName}
          StartReportGridDateFormatFunc={this.StartHandleDate}
          EndReportGridDateFormatFunc={this.EndHandleDate}
          ProjectDropDownValue={this.ProjectDropDownSelectedValue}
          HoursDaysDropDownValue={this.HoursDaysDropDownSelectedValue}
          HoursDaysDropDownValueShow={this.state.HoursDaysShow}
          RefreshCall={() => {
            this.setState(
              {
                IsUpdated: false,
                page: 0,
              },
              () => {
                this.GetCustomSearchValues();
                this.RefreshGridData();
                this.CustomColumnFilter();
              }
            );
          }}
          projectDDSelectedValue={this.state.ProjectIdFilter}
          projectDDSelectedValueShow={this.state.ProjectIdFilterShow}
          showStartDate={this.state.ShowStartDate}
          showEndDate={this.state.ShowEndDate}
          lstSubtask={this.state.lstSubtask}
          ShowSelectedDataAdmin={this.state.AdminReport}
          ClientList={this.state.lstClient}
          ClientIdChange={(e) => {
            this.ChangeClientID(e);
          }}
          ClientIDSelected={this.state.ClientIDSelected}
          SelectExportTypeFun={this.SelectExportType}
          lstDepartmentNames={this.state.lstDepartmentNames}
        />
        <ConfirmPopModal
          Title={this.state.Title}
          ShowMsgs={this.state.errorMsg}
          show={this.state.ConfrimMsgPopUp}
          onConfrim={this.CheckConfirm}
          onHide={this.CloseupdateModal}
        />
        <SureMsgPopUp
          Title={this.state.Title}
          ShowMsgs={this.state.errorMsg}
          show={this.state.ShowMsgPopUp}
          onConfrim={this.CheckPopUp}
          onHide={this.CloseReportModal}
        />
        <PopUpModal
          Title="Reports"
          errorMsgs={this.state.DateErrMsg} //"Start date cannot be greater than end date."
          show={this.state.ErrorModel}
          onHide={this.CloseModal}
        />
        <RecordUpdatePopUp
          Title={this.state.Title}
          errorMsgs={this.state.errorMsgs}
          show={this.state.RecordUpdatePopUpBit}
          onConfrim={this.CheckPopUpUpdate}
          onHide={this.CloseModalAfterUpdate}
        />
        <RecordAddedPopUp
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.RecordAddedPopUpBit}
          onHide={this.CloseModalAfterAdd}
        />
        {this.state.WBSExcelShow == true ? (
          <WBSExcelPopUpModal
            show={this.state.WBSExcelShow}
            lstProjectNames={this.state.lstProjectNames}
            onConfrim={() => {
              if (this.state.WBSExcelProjectID > 0) {
                this.WBSExcelExport();
                this.setState({
                  WBSExcelShow: false,
                  WBSExcelProjectID: -1,
                  WBSExcelProjectName: null,
                });
              }
            }}
            onHide={() => {
              this.setState({
                WBSExcelShow: false,
                WBSExcelProjectID: -1,
                WBSExcelProjectName: null,
              });
            }}
            WBSExcelProjectDropdown={(e) => {
              this.setState({
                WBSExcelProjectID:
                  e.target[e.target.selectedIndex].getAttribute("projectId"),
                WBSExcelProjectName: e.target.value,
              });
            }}
          />
        ) : null}
        {this.state.ExportToPdfModalShow ? (
          <ExportToPDFRefNumber
            ClientIDSelected={this.state.ClientIDSelected}
            ExportCheck={this.state.AdminReport.ExportType}
            GroupByProject={this.state.GroupByProject}
            show={this.state.ExportToPdfModalShow}
            ProjectList={this.state.GrouplstReport}
            pdftextboxvalueChanged={(e, index) => {
              if (this.state.GrouplstReport.length > 1) {
                let markers = [...this.state.GrouplstReport];
                markers[index] = {
                  ...markers[index],
                  ReferenceNumber: e.target.value,
                };
                this.setState({ GrouplstReport: markers });
              } else {
                this.setState({
                  pdftextboxvalue: e.target.value,
                });
              }

              // this.setState({
              //   pdftextboxvalue: e.target.value,
              // });
            }}
            pdftextboxvalueChangedGroupBy={(e, ClientID) => {
              this.state.GrouplstReport.map((a) => {
                if (a.ClientID === ClientID) {
                  a.ReferenceNumber = e.target.value;
                }
              });
            }}
            onHide={() => {
              this.setState({
                ExportToPdfModalShow: false,
                //pdftextboxvalue: null,
              });
            }}
            onSubmit={() => {
              this.setState(
                {
                  ExportToPdfModalShow: false,
                  // pdftextboxvalue: null,
                },
                () => {
                  this.ExportPdfReport();
                }
              );
            }}
          />
        ) : null}
        {this.state.UpdateRecordFields.AssignmentID ? (
          <UpdateAddRecord
            errorStyle={this.state.errorStyle}
            show={this.state.showUpdateModel}
            onHide={this.ConfrimModals}
            UpdateProjectData={this.state.UpdateRecordFields}
            UpdateHandleChange={(e) => {
              this.UpdateRecordHandleChange(e);
            }}
            UpdateProjectHandleChnage={this.UpdateProjectDropDownSelectedValue}
            UpdateActualDurationHandleChange={
              this.UpdateActualDurationHandleChange
            }
            DeleteRecord={() => {
              this.setState(
                {
                  IsUpdated: true,
                },
                () => {
                  this.ConfrimModal();
                }
              );
            }}
            UpdateStartHandleDate={this.UpdateStartHandleDate}
            UpdateRecord={this.UpdateCheckValidation}
            UpdateRecordCheckboxHandleFun={this.UpdateRecordCheckboxHandle}
            UpdateRecordsApproved={this.UpdateRecordsApproved}
            UpdateStat={(e) => {
              this.UpdateStateResource(e);
            }}
            UpdateProjectDropdown={(e) => {
              this.UpdateStateProject(e);
            }}
            EmptyProjectDropdown={this.EmptyProjectDropdown}
            RecordChange={this.UpdateRecordHandleChangeDropdown}
            SaveRecordUpdate={(e) => {
              this.setState({ SaveAssignment: e });
            }}
            //UpdateErrorStyle={this.state.errorStyle}
            //UpdateStyleRemoved={this.UpdateStyleRemoved}
            //UpdateProjectNames={this.state.lstAddRecord.Project}
            // UpdateProjectDate={moment(
            //   this.state.lstAddRecord.Date,
            //   "DD-MM-YYYY"
            // ).toDate()}
            // UpdateMainTaskName={this.state.lstAddRecord.MainTaskName}
            // UpdatMainTaskID={this.state.lstAddRecord.MainTaskID}
            // UpdateComment={this.state.lstAddRecord.SubTask}
            // UpdateActualDuration={this.state.lstAddRecord.Duration}
            // UpdateSubTask={this.state.lstAddRecord.Task}
            // UpdateID={this.state.lstAddRecord.ID}
            // UpdateProjectDescription={
            //   this.state.UpdateRecordFields.ProjectDescription
            // }
            // UpdateTaskOwnerName={this.state.lstAddRecord.TaskOwner}
            lstProjectNames={this.state.lstProjectNames}
            lstTaskOwnerName={this.state.lstTaskOwnerName}
            lstMainTaskName={this.state.lstMaintasksMapped}
            lstSubTaskName={this.state.lstSubtask}
          />
        ) : null}
        {/* {this.state.ExportShow == true ? (
          <Exportmodal
            ExportShow={this.state.ExportShow}
            Bill_Actual={this.state.Bill_Actual}
            ShowZeroHours={this.state.ShowZeroHours}
            Export_Type={this.state.Export_Type}
            Export_Media={this.state.Export_Media}
            CheckBoxFun={() => {
              if (this.state.ShowZeroHours == true) {
                this.setState({ ShowZeroHours: false });
              } else {
                this.setState({ ShowZeroHours: true });
              }
            }}
            Bill_ActualFun={(e) => {
              this.setState({ Bill_Actual: e.target.value }, () => {
                // console.log(this.state.Bill_Actual);
              });
            }}
            Export_TypeFun={(e) => {
              this.setState({ Export_Type: e.target.value }, () => {
                // console.log(this.state.Export_Type);
              });
            }}
            Export_MediaFun={(e) => {
              this.setState({ Export_Media: e.target.value }, () => {
                // console.log(this.state.Export_Media);
              });
            }}
            ExportonHide={() => {
              this.setState({
                ExportShow: false,
                Bill_Actual: null,
                ShowZeroHours: false,
                Export_Media: null,
              });
            }}
            onExport={this.onExportExcelPDF}
          />
        ) : null} */}
        {this.props.Refresh == true ? this.refreshGrid() : null}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    Refresh: state.RefreshReportDataBit,
  };
};
const mapDispatchToProps = (dispatch) => {
  return {
    NotRefreshReportAction: () => dispatch(NotRefreshReportAction()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ReportGrid);
