import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import axios from "axios";
import PopUpModal from "../PopUpMsgModal/PopUpModal";
import SureMsgPopUp from "./SureMsgPopUp";
import ConfirmPopModal from "./ConfirmPopUpModal";
import RecordAddedPopUp from "./RecordAddedPopUp";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "./DateCalander.css";
import Cookies from "js-cookie";
import { RefreshReportAction } from "../Redux/Actions/index";
import { connect } from "react-redux";
import moment from "moment";
import WarningPopup from "./RecordAddedPopUp";
import Loader from "../Loader/Loader";
const errorstyle = {
  borderStyle: "solid",
  borderWidth: "2px",
  borderColor: "Red",
};
class AddRecord extends Component {
  constructor(props) {
    super(props);
    this.state = {
      // lstReport: null,
      // GrouplstReport: null,
      // ProjectIdFilterShow: "",
      // ProjectIdFilter: null,
      //#region Add Records States
      showSelectedDates: "",
      ProjectName: "",
      TaskName: "",
      ActualDuration: "",
      Comment: "",
      MainTaskID: "",
      MainTaskName: "",
      lstMainTasks: null,
      TaskID: null,
      ResourceName: Cookies.get("UserName"),
      TaskOwnerID: null,
      //#endregion
      ProjectDescription: "",
      PhaseDescription: null,
      ProjectId: null,
      checkPopUp: "",
      ConfirmSave: true,
      ShowMsgPopUp: false,
      ConfrimMsgPopUp: false,
      PopUpBit: false,
      RecordAddedPopUpBit: false,
      UserIDassigned: null,
      UserNameAssigned: null,
      Title: "",
      errorMsg: "",
      errorStyle: {
        ProjectDD: "",
        ResourceNameDD: "",
        ProjectDescription: "",
        ActualDurationFiled: "",
        PhaseFiled: "",
        DescriptionFiled: "",
        CommentFiled: "",
      },
      BtnDisabled: false,
      SubTaskWarning: false,
      Loading: false,
      MainTaskError: false,
      UserError: false,
    };
  }

  componentDidMount() {
    //this.LoadReportsData();
    this.checkUser();
  }
  // LoadReportsData = () => {
  //   try {
  //     this.setState({ ShowFilterTop: true });
  //     this.setState({ Loading: true, GrouplstReport: [] });
  //     const objRptParam = {
  //       FromDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
  //       IsApproved: null,
  //       LoggedInUserId:
  //         Cookies.get("Role") === "Staff" ? Cookies.get("UserID") : -1,
  //       MainTaskID: -1,
  //       ProjectId:
  //         this.state.ProjectIdFilter === null ||
  //         this.state.ProjectIdFilter === ""
  //           ? -1
  //           : this.state.ProjectIdFilter, //"All",
  //       SubTaskID: -1,
  //       ToDate: "",
  //       ClientID: this.state.ClientIDSelected,
  //     };
  //     axios({
  //       method: "post",
  //       url: `${process.env.REACT_APP_BASE_URL}Reports/GetAssignments`,
  //       headers: {
  //         Authorization: "Bearer " + localStorage.getItem("access_token"),
  //       },

  //       data: objRptParam,
  //     }).then((res) => {
  //       var _tempCount = 0;
  //       var tempList = [];
  //       if (res.data.Result !== null) {
  //         res.data.Result.map((item) => {
  //           if (item.ProjectName === "TOTAL") {
  //             _tempCount = _tempCount + 0;
  //           } else {
  //             _tempCount++;
  //             tempList.push(item);
  //           }
  //         });
  //       }
  //       console.log(tempList);
  //       for (var i = 0; i <= 2; i++) {
  //         tempList.sort(function (a, b) {
  //           a = a.AssignmentDateTime.split("/");
  //           /* a[0] = day
  //           a[1] = month
  //           a[2] = year */
  //           var _tempa = new Date(a[2], a[1], a[0]);
  //           b = b.AssignmentDateTime.split("/");
  //           var _tempb = new Date(b[2], b[1], b[0]);
  //           return _tempb - _tempa;
  //         });
  //       }
  //       this.setState({ lstReport: tempList });
  //       this.setState({ ReportCount: _tempCount });
  //       this.setState({ Loading: false });
  //       var grouparray = [];
  //       for (var i = 0; i < res.data.Result.length; i++) {
  //         if (grouparray.length == 0) {
  //           var obj = {
  //             ProjectNameAll: res.data.Result[i]?.ProjectName,
  //             ProjectIdAll: res.data.Result[i]?.ProjectID,
  //           };
  //           grouparray.push(obj);
  //         } else {
  //           var CheckProject = grouparray.filter(
  //             (SName) => SName.ProjectIdAll === res.data.Result[i]?.ProjectID
  //           );
  //           if (CheckProject.length == 0) {
  //             var obj = {
  //               ProjectNameAll: res.data.Result[i]?.ProjectName,
  //               ProjectIdAll: res.data.Result[i]?.ProjectID,
  //             };
  //             grouparray.push(obj);
  //           }
  //         }
  //       }

  //       this.setState({ GrouplstReport: grouparray });
  //     });
  //   } catch (e) {
  //     window.location.href = "/Error";
  //   }
  // };

  CheckConfirm = () => {
    var gettaskownerID =
      this.state.TaskOwnerID === null
        ? Cookies.get("UserID")
        : this.state.TaskOwnerID;
    if (
      this.state.UserError == false &&
      Cookies.get("Role") === "SuperAdmin" &&
      gettaskownerID !== Cookies.get("UserID") &&
      (this.state.MainTaskID != "" || this.state.MainTaskID != null)
    ) {
      this.setState(
        { UserError: true, ConfrimMsgPopUp: false, MainTaskError: false },
        () => {
          // this.CheckDepartmentask(
          //   this.state.ProjectId,
          //   this.state.MainTaskID,
          //   gettaskownerID
          // );
        }
      );
    }

    if (this.state.UserError == true) {
      this.setState({
        //UserError: false,
        ConfirmSave: true,
        ConfrimMsgPopUp: false,
        ResourceName: this.state.UserNameAssigned,
        TaskOwnerID: this.state.UserIDassigned,
      });
    }
  };
  CloseModalAfterAdd = () => {
    if (this.state.Title == "Success") {
      // window.location.reload(false);
      this.props.RefreshReportAction(); //Set the redux state to true
      this.setState({ PopUpBit: false });
      this.setState({ ShowMsgPopUp: false });
      this.setState({ ConfrimMsgPopUp: false });
      this.setState({ RecordAddedPopUpBit: false });
      this.setState({ checkPopUp: "", errorMsg: "" });
      this.CloseAddRecord();
    }
    this.setState({ PopUpBit: false });
    this.setState({ ShowMsgPopUp: false });
    this.setState({
      ConfrimMsgPopUp: false,
      ProjectName: "",
      ResourceName: Cookies.get("UserName"),
      TaskOwnerID: null,
    });
    this.setState({ RecordAddedPopUpBit: false });
  };
  CloseModal = () => {
    if (this.state.Title == "Success") {
      // window.location.reload(false);
      this.props.RefreshReportAction(); //Set the redux state to true
      this.setState({ PopUpBit: false });
      this.setState({ ShowMsgPopUp: false });
      this.setState({ ConfrimMsgPopUp: false });
      this.setState({ RecordAddedPopUpBit: false });
      this.setState({ checkPopUp: "", errorMsg: "" });
      this.CloseAddRecord();
    }
    this.setState({ PopUpBit: false, BtnDisabled: false });
    this.setState({ ShowMsgPopUp: false, checkPopUp: "" });
    this.setState({ RecordAddedPopUpBit: false });
  };
  checkUser = () => {
    try {
      if (
        Cookies.get("Role") == "Admin" ||
        Cookies.get("Role") == "SuperAdmin"
      ) {
        return false;
      } else {
        return true;
      }
    } catch (ex) {
      return true;
    }
  };
  //#region  Events
  ReportProjectDDPopUpChange = (e) => {
    var _msg = e.target[e.target.selectedIndex].getAttribute("ddAttr");
    var _projectID =
      e.target[e.target.selectedIndex].getAttribute("ddAttrProjectID");
    var getloginusername = Cookies.get("UserName");
    var selectedUserName = this.state.ResourceName;
    this.setState(
      {
        ProjectDescription: _msg,
        ProjectId: _projectID,
        ProjectName: e.target.value,
        MainTaskID: "",
        MainTaskName: "",
        TaskID: "",
        TaskName: "",
      },
      () => {
        this.LoadMainTasks();
      }
    );
    if (
      Cookies.get("Role") === "SuperAdmin" &&
      getloginusername === selectedUserName
    ) {
      if (_msg !== null && _projectID !== null) {
        // this.setState(
        //   {
        //     ProjectDescription: _msg,
        //     ProjectId: _projectID,
        //     ProjectName: e.target.value,
        //   },
        //   () => {
        //     this.LoadMainTasks();
        //   }
        // );
      }
    } else {
      this.CheckAssignedUserWhenProjectSelected(e, _msg);
    }
  };
  CheckAssignedUserWhenProjectSelected = (e, _msg) => {
    try {
      var projectname = e.target.value;
      var projectid =
        e.target[e.target.selectedIndex].getAttribute("ddAttrProjectID");
      const CheckUsersData = {
        UserID:
          this.state.TaskOwnerID === null
            ? Cookies.get("UserID")
            : this.state.TaskOwnerID,
        ProjectID: projectid,
      };
      if (CheckUsersData.UserID && CheckUsersData.ProjectID) {
        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/CheckUserAssigned`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: CheckUsersData,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode === 200) {
            // this.setState({
            //   ProjectId: projectid,
            //   ProjectName: projectname,
            // });
            this.setState(
              {
                ProjectDescription: _msg,
                ProjectId: projectid,
                ProjectName: projectname,
              },
              () => {
                this.LoadMainTasks();
              }
            );
          }
          if (res.data.StatusCode === 404) {
            this.setState({
              Title: "Add Record",
              ConfirmSave: false,
              UserIDassigned: this.state.TaskOwnerID,
              UserNameAssigned: this.state.ResourceName,
            });
            this.setState({
              errorMsg:
                `Project is not assigned to ` +
                this.state.ResourceName +
                `. Do you still want to add?`,
            });
            this.setState({ ConfrimMsgPopUp: true });
          }
        });
      }
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  CheckAssignedUser = (e) => {
    try {
      var UserName = e.target.value;
      var userid = e.target[e.target.selectedIndex].getAttribute("taskOwnerID");
      const CheckUsersData = {
        UserID: userid,
        ProjectID: this.state.ProjectId,
      };
      if (CheckUsersData.UserID && CheckUsersData.ProjectID) {
        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}Reports/CheckUserAssigned`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: CheckUsersData,
        }).then((res) => {
          if (res.data?.StatusCode === 401) {
            this.logout();
          }
          if (res.data.StatusCode === 200) {
            this.setState({ ResourceName: UserName, TaskOwnerID: userid });
          }
          if (res.data.StatusCode === 404) {
            this.setState({
              Title: "Add Record",
              ConfirmSave: false,
              UserIDassigned: userid,
              UserNameAssigned: UserName,
              UserError: true,
            });
            this.setState({
              errorMsg:
                `Project is not assigned to ` +
                UserName +
                `. Do you still want to add?`,
            });
            this.setState({ ConfrimMsgPopUp: true });
          }
        });
      } else {
        this.setState({ ResourceName: UserName });
        this.setState(
          {
            TaskOwnerID: userid,
          },
          () => {
            var gettaskownerID =
              this.state.TaskOwnerID === null
                ? Cookies.get("UserID")
                : this.state.TaskOwnerID;
            if (
              Cookies.get("Role") === "SuperAdmin" &&
              gettaskownerID !==
                Cookies.get(
                  "UserID" &&
                    (this.state.MainTaskID != "" ||
                      this.state.MainTaskID != null)
                )
            ) {
              // this.CheckDepartmentask(
              //   this.state.ProjectId,
              //   this.state.MainTaskID,
              //   gettaskownerID
              // );
            }
          }
        );
      }
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  ReportResourceDDPopUpChange = (e) => {
    var getloginusername = Cookies.get("UserName");
    var selectedUserName = e.target.value;
    this.setState({ ResourceName: e.target.value });
    this.setState({
      TaskOwnerID: e.target[e.target.selectedIndex].getAttribute("taskOwnerID"),
    });
    if (
      Cookies.get("Role") === "SuperAdmin" &&
      getloginusername === selectedUserName
    ) {
      this.setState({ ConfirmSave: true });
      // this.setState({
      //   TaskOwnerID:
      //     e.target[e.target.selectedIndex].getAttribute("taskOwnerID"),
      // });
    } else {
      this.CheckAssignedUser(e);
    }
  };
  HandleChangeTask = (e) => {
    var target = e.target;
    var value = target.value;
    var maintaskids =
      e.target[e.target.selectedIndex].getAttribute("ddAttrMainTaskID");
    this.setState({ MainTaskID: maintaskids, Loading: true });
    this.setState({ MainTaskName: e.target.value });
    this.LoadPhaseDescription(this.state.ProjectId, maintaskids);

    var gettaskownerID =
      this.state.TaskOwnerID === null
        ? Cookies.get("UserID")
        : this.state.TaskOwnerID;
    if (
      Cookies.get("Role") === "SuperAdmin" &&
      gettaskownerID !== Cookies.get("UserID")
    ) {
      // this.CheckDepartmentask(
      //   this.state.ProjectId,
      //   maintaskids,
      //   gettaskownerID
      // );
    }
  };
  CheckDepartmentask = (Projectid, MainTaskId, gettaskownerID) => {
    try {
      const Obj = {
        ProjectID: Projectid,
        MainTaskID: MainTaskId,
        UserID: gettaskownerID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/CheckMainTask`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: Obj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 400) {
          this.setState({
            Title: "Add Record",
            ConfirmSave: false,
            MainTaskError: true,
          });
          this.setState({
            errorMsg:
              `Main Task is not assigned to ` +
              this.state.ResourceName +
              `. Do you still want to add?`,
          });
          this.setState({ ConfrimMsgPopUp: true });
        }
        //this.setState({ PhaseDescription: res.data.Result, Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  HandleChangeDescription = (e) => {
    try {
      var taskID = e.target[e.target.selectedIndex].getAttribute("taskID");
      this.setState({ TaskID: taskID });
      this.setState({ TaskName: e.target.value });
    } catch (ex) {
      //alert(ex);
    }
  };
  handleChangeEvent = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        [name]: value,
      });
    } catch (ex) {
      alert(ex);
    }
  };

  ActualDurationhandleChangeEvent = (e) => {
    try {
      const ActualDuration = e.target.validity.valid ? e.target.value : "";
      // this.state.ActualDuration;
      this.setState({ ActualDuration });
    } catch (ex) {
      alert(ex);
    }
  };

  //#endregion
  ShowCalender = () => {
    this.setState({ showDate: true });
  };
  CheckValidation = (e) => {
    var checkError = false;
    if (this.state.ProjectName == "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectDD: "input_error",
        },
      }));
    }
    if (this.state.ActualDuration == "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ActualDurationFiled: "input_error",
        },
      }));
    }
    if (parseFloat(this.state.ActualDuration) == 0) {
      checkError = true;
      this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Actual Duration cannot be equal to zero" });
      this.setState({ PopUpBit: true });
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ActualDurationFiled: "input_error",
        },
      }));
      e.preventDefault();
    }
    if (parseFloat(this.state.ActualDuration < 0)) {
      checkError = true;
      this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Actual Duration cannot be less then zero" });
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ActualDurationFiled: "input_error",
        },
      }));
      this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (parseFloat(this.state.ActualDuration) > 24) {
      checkError = true;
      this.setState({ Title: "Invalid Entry" });
      this.setState({
        errorMsg: "Actual Duration cannot be greater then 24 Hours",
      });
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ActualDurationFiled: "input_error",
        },
      }));
      this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.TaskName == "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          PhaseFiled: "input_error",
        },
      }));
    }
    if (this.state.TaskName == "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          DescriptionFiled: "input_error",
        },
      }));
    }
    if (this.state.Comment == "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          CommentFiled: "input_error",
        },
      }));
    }

    if (
      this.state.ProjectName !== "" ||
      this.state.MainTaskName !== "" ||
      this.state.TaskName !== ""
    ) {
      var dm = moment(this.props.AssignmentDateTime).format("DD/MM/YYYY");
      var checkTask = this.props.Reportlist.filter(
        (Name) =>
          Name.ProjectName.toLowerCase().includes(
            this.state.ProjectName.trim().toLowerCase()
          ) &&
          Name.UserFullName.toLowerCase().includes(
            this.state.ResourceName.trim().toLowerCase()
          ) &&
          Name.MainTaskName.toLowerCase().includes(
            this.state.MainTaskName.trim().toLowerCase()
          ) &&
          Name.TaskName.toLowerCase().includes(
            this.state.TaskName.trim().toLowerCase()
          ) &&
          Name.AssignmentDateTime.includes(dm)
      );

      if (checkTask.length > 0) {
        checkError = true;
        this.setState({ Title: "Alert" });
        this.setState({
          errorMsg: `Duplicate Record(s) not allowed`,
        });
        this.setState({ PopUpBit: true });
      }
    }
    /* if (this.state.ProjectDescription == "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectDescription: "input_error",
        },
      }));
    } */
    // if (!checkError) {
    //   this.setState({ BtnDisabled: true });
    //   this.setState({ Title: "Add Record" });
    //   this.setState({
    //     errorMsg: `Are you sure you want to add ${this.state.ActualDuration} Hour(s) in ${this.state.ProjectName}?`,
    //   });
    //   this.setState({ checkPopUp: "save" });
    //   this.setState({ ShowMsgPopUp: true });
    //   e.preventDefault();
    // }
    if (!checkError) {
      this.CheckUserHours(e);
    }
  };

  HandelErrorRemove = (name) => {
    if (name == "ProjectDD") {
      if (this.state.ProjectName == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectDD: "",
          },
        }));
      }
    }
    if (name == "ProjectDD") {
      if (this.state.ProjectName == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectDD: "",
          },
        }));
      }
    }
    if (name == "ActualDurationFiled") {
      if (this.state.ActualDuration == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ActualDurationFiled: "",
          },
        }));
      }
    }
    if (name == "PhaseFiled") {
      if (this.state.MainTaskName == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            PhaseFiled: "",
          },
        }));
      }
    }
    if (name == "DescriptionFiled") {
      if (this.state.TaskName == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            TaskFiled: "",
          },
        }));
      }
    }
    if (name == "CommentFiled") {
      if (this.state.Comment == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            CommentFiled: "",
          },
        }));
      }
    }
    if (name == "ProjectDescription") {
      if (this.state.ProjectDescription == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectDescription: "",
          },
        }));
      }
    }
  };

  ShowWarningMsg = (e) => {
    this.setState({ Title: "Add Record" });
    this.setState({ errorMsg: "Are you sure you want to exit?" });
    this.setState({ ShowMsgPopUp: true });
  };

  SaveRecords = (e) => {
    try {
      /* var RequestObj = {
        ID: "0", // it's 0 for save case later we will set proper value here.
        ProjectName: this.state.ProjectName,
        TaskName: this.state.TaskName,
        TaskOwnerName: this.state.ResourceName,
        AssignmentDateTime: this.props.AssignmentDateTime,
        ActualDuration: this.state.ActualDuration,
        Type: "Digital Marketing", // it's hardcoded in ems .net need to discuss it with faisal bhai
        SubTaskName: this.state.Comment,
        TaskType: "N", // hardcoded later we will change
        MainTaskID: this.state.MainTaskID,
      }; */
      var RequestObj = {
        ID: 0, // it's 0 for save case
        ProjectID: this.state.ProjectId,
        TASKID: this.state.TaskID,
        UserID:
          this.state.TaskOwnerID != null
            ? this.state.TaskOwnerID
            : Cookies.get("UserID"), //Sending UserID gET TaskOwnerID In SP
        AssignmentDateTime: this.props.AssignmentDateTime,
        ActualDuration: this.state.ActualDuration,
        CommentText: this.state.Comment,
        MainTaskID: this.state.MainTaskID,
      };
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
        if (res.data.StatusCode == 200) {
          this.setState({ Title: "Success" });
          this.setState({ errorMsg: "Record Save Successfully" });
          this.setState({ RecordAddedPopUpBit: true, BtnDisabled: false });
          this.setState({ showSelectedDate: false });
          // this.LoadReportsData();
        }
        if (res.data.StatusCode == 400 && res.data.MainTaskCheck == true) {
          this.setState({
            Title: "Error",
            ShowMsgPopUp: false,
            checkPopUp: "",
          });
          this.setState({ errorMsg: res.data.Result });
          this.setState({ SubTaskWarning: true, BtnDisabled: false });
          this.setState({ showSelectedDate: false });
        }
      });
      //this.LoadReportsData();
    } catch (ex) {
      this.setState({ BtnDisabled: false });
      window.location.href = "/Error";
    }
  };
  LoadMainTasks = () => {
    try {
      this.setState({
        Loading: true,
      });
      const ProjectId = {
        ProjectID: this.state.ProjectId,
        UserID:
          Cookies.get("Role") == "SuperAdmin"
            ? 0
            : this.state.TaskOwnerID != null
            ? this.state.TaskOwnerID
            : Cookies.get("UserID"),
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
        this.setState({ lstMaintasks: res.data.Result, Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadPhaseDescription = (Projectid, MainTaskId) => {
    try {
      const Obj = {
        projectID: Projectid,
        maintaskID: MainTaskId,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}TasksDescription/GetTaskDescription`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: Obj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ PhaseDescription: res.data.Result }, () => {
          this.setState(
            {
              TaskID: res.data.Result[0]?.taskTableID,
              TaskName: res.data.Result[0]?.taskName,
            },
            () => {
              this.setState({ Loading: false });
            }
          );
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  CloseAddRecord = () => {
    this.setState({ ShowMsgPopUp: false });
    this.setState({ ProjectDescription: "" });
    this.setState({ ProjectName: "" });
    this.setState({ TaskOwnerID: null });
    this.setState({ TaskName: "" });
    this.setState({ MainTaskName: "" });
    this.setState({ Comment: "" });
    this.setState({ MainTaskID: "" });
    this.setState({ ProjectId: null });
    this.setState({ PhaseDescription: null });
    this.setState({ ActualDuration: "" });
    this.setState({ errorStyle: {} });
    this.setState({
      ProjectId: null,
      ConfrimMsgPopUp: false,
      ProjectName: "",
      ResourceName: Cookies.get("UserName"),
      TaskOwnerID: Cookies.get("UserID"),
    });
    this.setState({ RecordAddedPopUpBit: false });
    this.props.hide();
  };
  CheckPopUp = () => {
    if (this.state.checkPopUp == "save") {
      if (this.state.ConfirmSave) {
        this.SaveRecords();
      }
    } else {
      this.CloseAddRecord();
    }
  };
  StyleRemoved = () => {
    this.setState({ errorStyle: {} });
  };

  CheckUserHours = (e) => {
    try {
      const CheckUsersData = {
        UserID:
          Cookies.get("Role") == "Admin" || Cookies.get("Role") == "SuperAdmin"
            ? this.state.TaskOwnerID == null
              ? Cookies.get("UserID")
              : this.state.TaskOwnerID
            : Cookies.get("UserID"),
        ProjectID: this.state.ProjectId,
        DateTime: this.props.showSelectedDates,
        Hours: this.state.ActualDuration,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/CheckUserHours`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: CheckUsersData,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.Result > 24) {
          this.setState({ Title: "Alert" });
          this.setState({
            errorMsg: "Record is not saved. 24 Hours Limit Exceeded.",
          });
          this.setState({ SubTaskWarning: true, BtnDisabled: false });
          this.setState({ showSelectedDate: false });
        }
        if (res.data.Result <= 24) {
          this.setState({ BtnDisabled: true });
          this.setState({ Title: "Add Record" });
          this.setState({
            errorMsg: `Are you sure you want to add ${this.state.ActualDuration} Hour(s) in ${this.state.ProjectName}?`,
          });
          this.setState({ checkPopUp: "save" });
          this.setState({ ShowMsgPopUp: true });
          e.preventDefault();
        }
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  render() {
    var ProjectNameList = null;

    if (
      /*  Cookies.get("Role") === "Staff" && */
      this.props.ProjectNameMapped.length > 0 &&
      this.props.Name !== null
    ) {
      if (this.props.ProjectNameMapped.length > 0) {
        ProjectNameList =
          this.props.Name !== null ? (
            this.props.Name.map((item) => {
              for (var y = 0; y < this.props.ProjectNameMapped.length; y++) {
                if (this.props.ProjectNameMapped[y].ProjectId === item.ID) {
                  return (
                    <option
                      ddAttrProjectID={item.ID}
                      ddAttr={item.ProjectDescription}
                      value={item.Name}
                    >
                      {item.Name}
                    </option>
                  );
                }
              }
            })
          ) : (
            <option> Loading </option>
          );
      }
    } /* else {
      ProjectNameList =
        this.props.Name !== null ? (
          this.props.Name.map((item) => {
            return (
              <option
                ddAttrProjectID={item.ID}
                ddAttr={item.ProjectDescription}
                value={item.Name}
              >
                {item.Name}
              </option>
            );
          })
        ) : (
          <option> Loading </option>
        );
    } */

    var MainTaskList =
      this.state.lstMaintasks == null
        ? ""
        : this.state.lstMaintasks.map((item) => {
            /* var ProjectIdFromTask = item.ProjectIDs; */
            if (
              this.state.ProjectID !== null &&
              item.MainTaskName !== "No MainTask"
            ) {
              /* var splitArr = ProjectIdFromTask.split(",");
              for (var i = 0; i < splitArr.length; i++) {
                if (this.state.ProjectId == splitArr[i]) {
                  return (
                    <option
                      ddAttrMainTaskID={item.Id}
                      value={item.MainTaskName}
                    >
                      {item.MainTaskName}
                    </option>
                  );
                }
              } */ return (
                <option ddAttrMainTaskID={item.Id} value={item.MainTaskName}>
                  {item.MainTaskName}
                </option>
              );
            }
          });

    /*     var TaskOwnerNameList =
      this.props.TaskOwnerNameRecord == null
        ? ""
        : this.props.TaskOwnerNameRecord.map((item) => {
            return (
              <option value={item.TaskOwnerName}>{item.TaskOwnerName}</option>
            );
          }); */
    var TaskOwnerNameList =
      this.props.TaskOwnerNameRecord == null
        ? ""
        : this.props.TaskOwnerNameRecord.map((item) => {
        if(item?.Status===true){
          return (
            <option value={item.Name} taskOwnerID={item.UserProfileTableID} >
              {item.Name}
            </option>
          );
        }
            
          });

    var TaskNameList =
      this.state.PhaseDescription == null
        ? ""
        : this.state.PhaseDescription.map((item, index) => {
            return (
              <option value={item.taskName} taskID={item.taskTableID}>
                {item.taskName}
              </option>
            );
          });

    return (
      <>
        {/* {this.state.Loading ? (
        <Loader />
      ) : ( */}
        <>
          <Modal
            show={this.props.show}
            onHide={this.ShowWarningMsg}
            backdrop={"static"}
            keyboard={"false"}
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered
          >
            <div
              class="modal-content add_record_modal"
              style={{ boxShadow: "1px 5px 17px 5px #acabb1" }}
            >
              <Modal.Header closeButton>
                <Modal.Title>Add Record</Modal.Title>
              </Modal.Header>
              <Modal.Body>
                <div className="col-md-12 material_style pt-3">
                  <form className="row">
                    <div className="col-sm-6">
                      <div
                        className={
                          "form-group Select_label styled-select " +
                          this.state.errorStyle.ProjectDD
                        }
                      >
                        <select
                          //style={this.state.errorStyle.ProjectDD}
                          //onFocus={this.StyleRemoved}
                          onFocus={() => {
                            this.HandelErrorRemove("ProjectDD");
                          }}
                          className="form-control "
                          id="projectDropdown"
                          onChange={this.ReportProjectDDPopUpChange}
                          required
                          value={this.state.ProjectName}
                        >
                          <option value="" disabled selected hidden>
                            {/* --Select-- */}
                          </option>
                          {ProjectNameList}
                        </select>
                        <label>Project</label>
                      </div>
                    </div>
                    <div className="col-sm-6">
                      <div
                        /* className="form-group Select_label styled-select" */
                        className={
                          "form-group Select_label styled-select " +
                          this.state.errorStyle.ResourceNameDD
                        }
                      >
                        <select
                          //style={this.state.errorStyle.ResourceNameDD}
                          // onFocus={}
                          onFocus={() => {
                            this.StyleRemoved();
                            this.HandelErrorRemove("ResourceNameDD");
                          }}
                          className="form-control"
                          id="resourceDropdown"
                          /*   readOnly={this.checkUser()}
                      disabled={this.checkUser()} */
                          onChange={this.ReportResourceDDPopUpChange}
                          required
                          value={this.state.ResourceName}
                        >
                          {/* <option value={this.state.ResourceName} select hidden>
                        {this.state.ResourceName}
                      </option> */}

                          {TaskOwnerNameList}
                        </select>
                        <label>Resource</label>
                      </div>
                    </div>
                    <div className="col-sm-12">
                      <div
                        /* className="form-group  textarea_mbl" */
                        className={
                          this.state.ProjectDescription == ""
                            ? "form-group  textarea_mbl " +
                              this.state.errorStyle.ProjectDescription
                            : "form-group  textarea_mbl txtArea_value_added " +
                              this.state.errorStyle.ProjectDescription
                        }
                      >
                        <textarea
                          className="ui-widget form-control txtArea text_boxshadow"
                          readOnly={true}
                          value={this.state.ProjectDescription}
                          style={{ resize: "none" }}
                          id="txtareaProjectDescription"
                          rows="3"
                          cols="50"
                          required
                          onFocus={() => {
                            this.HandelErrorRemove("ProjectDescription");
                          }}
                        >
                          {this.state.ProjectDescription}
                        </textarea>
                        <label class="modal_label" style={{ width: "inherit" }}>
                          Project Description
                        </label>
                      </div>
                    </div>
                    <div className="col-sm-6">
                      <div
                        className={
                          this.props.showSelectedDates == null
                            ? "form-group date_value_added"
                            : "form-group "
                        }
                      >
                        <div className="customDatePickerWidth">
                          {Cookies.get("Role") === "Staff" ||
                          Cookies.get("Role") === "Admin" ? (
                            <DatePicker
                              //placeholderText={"DD/MM/YYYY"}
                              dateFormat="dd/MM/yyyy"
                              minDate={new Date()}
                              maxDate={new Date()}
                              selected={this.props.showSelectedDates}
                              shouldCloseOnSelect={true}
                              className="form-control"
                              onChange={this.props.AssignmentDateTimes}
                              required
                              autoComplete="off"
                              readOnly
                            />
                          ) : (
                            <DatePicker
                              maxDate={new Date()}
                              dateFormat="dd/MM/yyyy"
                              selected={this.props.showSelectedDates}
                              shouldCloseOnSelect={true}
                              className="form-control"
                              onChange={this.props.AssignmentDateTimes}
                              required
                              autoComplete="off"
                            />
                          )}

                          <label className="datepicker_label">Date</label>
                        </div>
                      </div>
                    </div>
                    <div className="col-sm-6">
                      <div
                        className={
                          "form-group addrecordinput_label " +
                          this.state.errorStyle.ActualDurationFiled
                        }
                      >
                        <input
                          className="form-control "
                          name="ActualDuration"
                          id="durationInput"
                          //placeholder="Up to 8"
                          type="text"
                          pattern="[0-9.]*"
                          onInput={this.ActualDurationhandleChangeEvent.bind(
                            this
                          )}
                          value={this.state.ActualDuration}
                          //style={this.state.errorStyle.ActualDurationFiled}
                          //onFocus={this.StyleRemoved}
                          onFocus={() => {
                            this.HandelErrorRemove("ActualDurationFiled");
                          }}
                          required
                          autoComplete="off"
                        />
                        <label>Actual Duration</label>
                      </div>
                    </div>
                    <div className="col-sm-6">
                      <div
                        //className="form-group Select_label styled-select"
                        className={
                          "form-group Select_label styled-select " +
                          this.state.errorStyle.PhaseFiled
                        }
                      >
                        <select
                          onChange={this.HandleChangeTask}
                          className="ui-widget form-control chosen"
                          id="MaintasksDropdown"
                          //onchange="ValidateProjectDropdown()"
                          onFocus={() => {
                            this.HandelErrorRemove("PhaseFiled");
                          }}
                          //style={this.state.errorStyle.PhaseFiled}
                          //onFocus={this.StyleRemoved}
                          required
                          value={this.state.MainTaskName}
                        >
                          <option value="" disabled selected hidden>
                            {/*  --Select-- */}
                          </option>
                          {MainTaskList}
                        </select>
                        <label>Main Task</label>
                      </div>
                    </div>
                    <div className="col-sm-6">
                      <div
                        //className="form-group Select_label styled-select"
                        className={
                          "form-group Select_label styled-select " +
                          this.state.errorStyle.DescriptionFiled
                        }
                      >
                        <select
                          className=" ui-widget form-control chosen"
                          id="tasksDropdown"
                          onClick={this.HandleChangeDescription}
                          onSelect={this.HandleChangeDescription}
                          onChange={this.HandleChangeDescription}
                          //style={this.state.errorStyle.DescriptionFiled}
                          //onFocus={this.StyleRemoved}
                          onFocus={() => {
                            this.HandelErrorRemove("DescriptionFiled");
                          }}
                          required
                          value={this.state.TaskName}
                        >
                          <option value="" disabled selected hidden>
                            {/*  --Select-- */}
                          </option>
                          {TaskNameList}
                        </select>
                        <label>Task/PBI</label>
                      </div>
                    </div>
                    <div className="col-sm-12">
                      <div
                        //className="form-group textarea_mbl"
                        className={
                          "form-group textarea_mbl " +
                          this.state.errorStyle.CommentFiled
                        }
                      >
                        <textarea
                          className="ui-widget form-control txtArea text_boxshadow"
                          name="Comment"
                          onChange={this.handleChangeEvent}
                          //placeholder="Explain the task done in detail. Include project name for which the task was done"
                          id="txtSubTaskArea"
                          rows="3"
                          cols="50"
                          style={{ resize: "none", margin: 4.5 }}
                          //style={this.state.errorStyle.CommentFiled}
                          //onFocus={this.StyleRemoved}
                          onFocus={() => {
                            this.HandelErrorRemove("CommentFiled");
                          }}
                          required
                        ></textarea>
                        <label class="modal_label">Description</label>
                      </div>
                    </div>
                  </form>
                </div>
              </Modal.Body>
              <Modal.Footer>
                <Button
                  className="btn-black"
                  onClick={this.CheckValidation}
                  disabled={this.state.BtnDisabled}
                >
                  Save
                </Button>
                <Button className="btn-black" onClick={this.ShowWarningMsg}>
                  Cancel
                </Button>
              </Modal.Footer>
              <PopUpModal
                Title={this.state.Title}
                errorMsgs={this.state.errorMsg}
                show={this.state.PopUpBit}
                onHide={this.CloseModal}
              />
              <SureMsgPopUp
                Title={this.state.Title}
                ShowMsgs={this.state.errorMsg}
                show={this.state.ShowMsgPopUp}
                onConfrim={this.CheckPopUp}
                onHide={this.CloseModal}
              />
              <RecordAddedPopUp
                Title={this.state.Title}
                errorMsgs={this.state.errorMsg}
                show={this.state.RecordAddedPopUpBit}
                onHide={this.CloseModalAfterAdd}
              />
              <WarningPopup
                Title={this.state.Title}
                errorMsgs={this.state.errorMsg}
                show={this.state.SubTaskWarning}
                onHide={() => {
                  this.setState({
                    Title: "",
                    errorMsg: "",
                    SubTaskWarning: false,
                  });
                }}
              />
              <ConfirmPopModal
                Title={this.state.Title}
                ShowMsgs={this.state.errorMsg}
                show={this.state.ConfrimMsgPopUp}
                onConfrim={this.CheckConfirm}
                onHide={() => {
                  if (this.state.MainTaskError) {
                    this.setState({
                      MainTaskName: "",
                      MainTaskError: false,
                      ConfrimMsgPopUp: false,
                      MainTaskID: "",
                    });
                  } else {
                    this.setState(
                      {
                        UserError: false,
                        MainTaskError: false,
                        ProjectId: null,
                        ConfrimMsgPopUp: false,
                        ProjectName: "",
                        ResourceName:
                          this.state.ResourceName !== null
                            ? this.state.ResourceName
                            : Cookies.get("UserName"),
                        TaskOwnerID:
                          this.state.TaskOwnerID !== null
                            ? this.state.TaskOwnerID
                            : Cookies.get("UserID"),
                      },
                      () => {
                        this.CloseModal();
                      }
                    );
                  }
                }}
              />
            </div>
          </Modal>
          {this.state.Loading ? <Loader /> : null}
        </>
        {/* )} */}
      </>
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
    RefreshReportAction: () => dispatch(RefreshReportAction()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(AddRecord);
