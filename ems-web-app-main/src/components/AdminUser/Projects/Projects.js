import React, { Component } from "react";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import PopUpModal from "../PopUpModalAdmin/PopUpModal";
import Table from "@material-ui/core/Table";
import TableContainer from "@material-ui/core/TableContainer";
import TablePagination from "@material-ui/core/TablePagination";
import moment from "moment";
import styled from "styled-components";
import AddProjectPopUp from "./AddProjectPopUp";
import UpdateProjectPopUp from "./UpdateProjectPopUp";
import Cookies from "js-cookie";
import { Link } from "react-router-dom";
import Tooltip from "@material-ui/core/Tooltip";
import InfiniteScroll from "react-infinite-scroll-component";
import { Dropdown, DropdownButton } from "react-bootstrap";
import Button from "react-bootstrap/Button";
import ButtonGroup from "react-bootstrap/ButtonGroup";
import "./AddProject.css";
import NoPic from "../../../images/profile-avator.png";
import Loader from "../../Loader/Loader";
import SureMsgPopUp from "../Tasks/MainTask/SureMsgPopUp";
import { encrypt, decrypt } from "react-crypt-gsm";
import FilterModal from "./FilterModal";
const Tablestyle = styled.td`
  padding-top: 0 !important;
  padding-bottom: 0 !important;
`;

/* const errorstyle = {
  borderStyle: "solid",
  borderWidth: "2px",
  borderColor: "Red",
}; */
class Projects extends Component {
  constructor(props) {
    super(props);
    this.state = {
      lstProjectData: null,
      lstProjectDataAll: null,
      ProjectId: -1,
      SelectdDate: new Date(),
      ShowSelectedDate: null,
      datePickerIsOpen: false,
      StartDate: null,
      EndDate: null,
      dataFormat: null,
      lstProjectDataCount: 0,
      ProjectData: null,

      PopUpBit: false,
      Title: "",
      errorMsg: "",
      page: 0,
      rowsPerPage: 10,
      iconChange: "fa fa-th-list",
      AddProjectPopBit: false,
      UpdateProjectPopBit: false,
      ImageModalPopBit: false,
      monthArray: [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
      ],
      monthBudget: {
        January: 0,
        February: 0,
        March: 0,
        April: 0,
        May: 0,
        June: 0,
        July: 0,
        August: 0,
        September: 0,
        October: 0,
        November: 0,
        December: 0,
      },

      AddProjectField: {
        ProjectID: -1,
        ProjectName: "",
        StartDate: null,
        EndDate: null,
        Status: "",
        TaskOwner: "",
        ProjectType: "",
        ProjectDescription: "",
        Client: null,
      },
      /* errorStyle: {
        ProjectName: null,
        StartDate: null,
        EndDate: null,
        TaskOwner: null,
        ProjectType: null,
        ProjectDescription: null,
      }, */
      errorStyle: {
        ProjectName: "",
        StartDate: "",
        EndDate: "",
        TaskOwner: null,
        ProjectType: "",
        Status: "",
        ProjectDescription: "",
        Client: null,
      },
      UpdateDataShow: null,
      currentBudgetYear: new Date(),
      lstProjectBudgetDetail: null,
      SearchProject: null,
      lstSearchProject: null,
      DayHourToggle: {
        dayStyle: "btn_underline",
        hourStyle: "",
      },
      ProjectLazyLoading: {
        ProjectScrollIndex: 1,
        ProjectRecoardPerPage: 20,
        CurrentProjectDataCount: null,
        ShowProjectList: false,
        ProjectHasMore: true,
      },
      ShowProjectName: "All Project",
      Loading: true,
      Filtermodal: false,
      FilterStatus: null,
      SearchProjectID: -1,
      SearchFilterStatus: null,
    };
  }
  LoadProjectNameLazyLoading = () => {
    try {
      const obj = {
        page: this.state.ProjectLazyLoading.ProjectScrollIndex,
        recsPerPage: this.state.ProjectLazyLoading.ProjectRecoardPerPage,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsLazyLoading`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: obj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ ProjectData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  handleMonthBudgetProject = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        monthBudget: {
          ...this.state.monthBudget,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  handleAddProject = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        AddProjectField: {
          ...this.state.AddProjectField,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  handleStartDateChange = (date) => {
    this.setState(
      {
        AddProjectField: {
          ...this.state.AddProjectField,
          StartDate: date,
        },
      },
      () => {
        if (this.state.AddProjectField?.EndDate) {
          var arr = [];
          var dt = moment(
            new Date(this.state.AddProjectField.StartDate)
          ).format("MM");
          var tempStartDate = this.state.AddProjectField.StartDate;
          var st = moment(new Date(this.state.AddProjectField.StartDate));
          var et = moment(new Date(this.state.AddProjectField.EndDate));
          var months = et.diff(st, "months");
          let count = 0;
          if (moment(st).format("YYYY") == moment(et).format("YYYY")) {
            while (count <= months) {
              arr.push(moment(new Date(tempStartDate)).format("MMMM"));
              tempStartDate = moment(tempStartDate).add(1, "M");
              ++dt;
              count++;
            }
          } else {
            var tempDate = new Date(st);
            var startdatevaue = `${tempDate.getFullYear()}-${1}-${1}`;
            while (count < 12) {
              arr.push(moment(new Date(startdatevaue)).format("MMMM"));
              startdatevaue = moment(startdatevaue).add(1, "M");
              ++dt;
              count++;
            }
          }
          this.setState({ monthArray: arr });
        }
      }
    );
  };
  handleEndDateChange = (date) => {
    this.setState(
      {
        AddProjectField: {
          ...this.state.AddProjectField,
          EndDate: date,
        },
      },
      () => {
        var arr = [];
        var dt = moment(new Date(this.state.AddProjectField.StartDate)).format(
          "MM"
        );
        var st = moment(new Date(this.state.AddProjectField.StartDate));
        var et = moment(new Date(this.state.AddProjectField.EndDate));
        var months = et.diff(st, "months");
        var tempStartDate = this.state.AddProjectField.StartDate;
        let count = 0;

        if (moment(st).format("YYYY") == moment(et).format("YYYY")) {
          while (count <= months) {
            arr.push(moment(new Date(tempStartDate)).format("MMMM"));
            tempStartDate = moment(tempStartDate).add(1, "M");
            ++dt;
            count++;
          }
        } else {
          var tempDate = new Date(st);
          var startdatevaue = `${tempDate.getFullYear()}-${1}-${1}`;
          while (count < 12) {
            arr.push(moment(new Date(startdatevaue)).format("MMMM"));
            startdatevaue = moment(startdatevaue).add(1, "M");
            ++dt;
            count++;
          }
        }

        this.setState({ monthArray: arr });
      }
    );
  };
  handleSearchProjectChange = (e) => {
    this.setState(
      {
        SearchProject: e.target.value,
      },
      () => {
        if (this.state.SearchProject != "") {
          this.SearchProject();
        } else {
          this.setState({ lstSearchProject: null }, () => {
            this.LoadProjectGrid();
          });
        }
      }
    );
  };
  CheckValidationAddProject = (e) => {
    var checkError = false;
    if (this.state.AddProjectField.ProjectName == "") {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Project Name" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectName: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectName: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.ProjectName !== "") {
      var checkTask = this.state.ProjectData.filter((Name) =>
        Name.Name.toLowerCase().includes(
          this.state.AddProjectField?.ProjectName.trim().toLowerCase()
        )
      );
      if (checkTask.length > 0) {
        checkError = true;
        this.setState({ Title: "Project Alert" });
        this.setState({
          errorMsg:
            `Project Name "` +
            this.state.AddProjectField?.ProjectName +
            `" Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask.length > 1) {
          checkError = true;
          this.setState({ Title: "Project Alert" });
          this.setState({
            errorMsg:
              `Project Name "` +
              this.state.AddProjectField?.ProjectName +
              `" Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
    }
    if (this.state.AddProjectField.StartDate == null) {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({
       errorMsg: "Please enter Start Date",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          StartDate: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          StartDate: "input_error",
        },
      }));
      // this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.EndDate == null) {
      /*  this.setState({ Title: "Invalid Entry" });
      this.setState({
       errorMsg: "Please enter Start Date",
       });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          EndDate: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          EndDate: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    // if (this.state.AddProjectField.TaskOwner === "") {
    //   /*  this.setState({ Title: "Invalid Entry" });
    //   this.setState({ errorMsg: "Please enter Task Owner Name" });
    //   this.setState({
    //     errorStyle: {
    //       ...this.state.errorStyle,
    //       TaskOwner: errorstyle,
    //     },
    //   }); */
    //   checkError = true;
    //   this.setState((prevState) => ({
    //     errorStyle: {
    //       ...prevState.errorStyle,
    //       TaskOwner: "input_error",
    //     },
    //   }));
    //   //this.setState({ PopUpBit: true });
    //   e.preventDefault();
    // }
    if (this.state.AddProjectField.ProjectType == "") {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please select Project Type" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectType: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectType: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.Status === "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          Status: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.ProjectDescription == "") {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Project Description" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectDescription: errorstyle,
        },
      }); */

      // Wasii Hide
      // checkError = true;
      // this.setState((prevState) => ({
      //   errorStyle: {
      //     ...prevState.errorStyle,
      //     ProjectDescription: "input_error",
      //   },
      // }));

      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.Client == null) {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          Client: "input_error",
        },
      }));
    }
    if (!checkError) {
      this.addProject();
    }
  };
  CheckPopUp = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CloseModal1 = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  HandelErrorRemove = (name) => {
    if (name == "ProjectName") {
      if (this.state.AddProjectField.ProjectName == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectName: "",
          },
        }));
      }
    }

    if (name == "StartDate") {
      if (this.state.AddProjectField.StartDate == null) {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            StartDate: "",
          },
        }));
      }
    }
    if (name == "EndDate") {
      if (this.state.AddProjectField.EndDate == null) {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            EndDate: "",
          },
        }));
      }
    }
    if (name == "TaskOwner") {
      if (this.state.AddProjectField.TaskOwner === "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            TaskOwner: "",
          },
        }));
      }
    }
    if (name == "ProjectType") {
      if (this.state.AddProjectField.ProjectType == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectType: "",
          },
        }));
      }
    }
    if (name == "Status") {
      if (this.state.AddProjectField.Status == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            Status: "",
          },
        }));
      }
    }
    if (name == "ProjectDescription") {
      if (this.state.AddProjectField.ProjectDescription == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectDescription: "",
          },
        }));
      }
    }
    if (name == "Client") {
      if (this.state.AddProjectField.Client == null) {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            Client: "",
          },
        }));
      }
    }
  };

  CheckValidationUpdateProject = (e) => {
    var checkError = false;
    if (this.state.AddProjectField.ProjectName == "") {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Project Name" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectName: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectName: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.StartDate == null) {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({
       errorMsg: "Please enter Start Date",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          StartDate: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          StartDate: "input_error",
        },
      }));
      // this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.EndDate == null) {
      /*  this.setState({ Title: "Invalid Entry" });
      this.setState({
       errorMsg: "Please enter Start Date",
       });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          EndDate: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          EndDate: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }

    // if (this.state.AddProjectField.TaskOwner === "") {
    //   /*  this.setState({ Title: "Invalid Entry" });
    //   this.setState({ errorMsg: "Please enter Task Owner Name" });
    //   this.setState({
    //     errorStyle: {
    //       ...this.state.errorStyle,
    //       TaskOwner: errorstyle,
    //     },
    //   }); */
    //   checkError = true;
    //   this.setState((prevState) => ({
    //     errorStyle: {
    //       ...prevState.errorStyle,
    //       TaskOwner: "input_error",
    //     },
    //   }));
    //   //this.setState({ PopUpBit: true });
    //   e.preventDefault();
    // }
    if (this.state.AddProjectField.ProjectType == "") {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please select Project Type" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectType: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectType: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }

    if (this.state.AddProjectField.Status === "") {
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          Status: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.ProjectDescription == "") {
      /* this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Project Description" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectDescription: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          ProjectDescription: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
    if (this.state.AddProjectField.ProjectName !== "") {
      var checkTask = this.state.ProjectData.filter((Name) =>
        Name.Name.toLowerCase().includes(
          this.state.AddProjectField?.ProjectName.trim().toLowerCase()
        )
      );
      if (
        checkTask.length > 0 &&
        checkTask[0]?.ID !== this.state.AddProjectField?.ProjectID
      ) {
        checkError = true;
        this.setState({ Title: "Project Alert" });
        this.setState({
          errorMsg:
            `Project Name "` +
            this.state.AddProjectField?.ProjectName +
            `" Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask.length > 1) {
          checkError = true;
          this.setState({ Title: "Project Alert" });
          this.setState({
            errorMsg:
              `Project Name "` +
              this.state.AddProjectField?.ProjectName +
              `" Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
    }
    if (!checkError) {
      this.updateProject();
    }
  };
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
    var tempDate = moment(new Date()).format("MMMM,YYYY");
    this.setState({ ShowSelectedDate: tempDate });
    this.LoadProjectGrid();
    /* this.LoadProjectName(); */
    this.LoadProjectNameLazyLoading();
  }
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
  LoadProjectGrid = () => {
    try {
      this.setState({ Loading: true });
      var StartDate = moment(this.state.StartDate).format("YYYY/MM/DD");
      var EndDate = moment(this.state.EndDate).format("YYYY/MM/DD");
      var ProjectIdAsInt = -1;

      if (this.state.ProjectId > -1) {
        ProjectIdAsInt = parseInt(this.state.ProjectId, 10);
      } else if (this.state.lstSearchProject != null) {
        ProjectIdAsInt = parseInt(this.state.lstSearchProject[0].ProjectID, 10);
      }

      const AdminDataObj = {
        RequestStartDate: StartDate,
        RequestEndDate: EndDate,
        ProjectID: ProjectIdAsInt,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/AllProject`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AdminDataObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstProjectData: res.data.Result,
          lstProjectDataAll: res.data.Result,
        });
        this.setState({ lstProjectDataCount: res.data.Result.length });
        if (this.state.StartDate != null) {
          var tempStartDate = moment(this.state.StartDate).format("MMMM, YYYY");
          var tempEndDate = moment(this.state.EndDate).format("MMMM, YYYY");

          this.setState({
            ShowSelectedDate: tempStartDate + "  to  " + tempEndDate,
          });

          this.setState({ datePickerIsOpen: false });
        } else {
          var tempDate = moment(new Date()).format("MMMM,YYYY");
          this.setState({ ShowSelectedDate: tempDate });
        }

        if (this.state.FilterStatus != null && this.state.FilterStatus != "") {
          this.SearchProjectGrid();
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      //alert(e);
    }
  };
  /*   LoadProjectName = () => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: "",
      }).then((res) => {
        this.setState({ ProjectData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  }; */
  SelectdDateHandler = (dates) => {
    const [start, end] = dates;
    if (start != null) {
      this.setState({ StartDate: start });
    }
    if (end == null) {
      this.setState({ EndDate: null });
    }

    if (end != null) {
      this.setState({ EndDate: end }, () => {
        var tempStartDate = moment(this.state.StartDate).format("YYYY");
        var tempEndDate = moment(this.state.EndDate).format("YYYY");
        if (tempStartDate - tempEndDate == 0) {
          this.LoadProjectGrid();
        } else {
          this.setState({ Title: "Error" });
          this.setState({
            errorMsg: "Start Date and End Date must be in same Year!!!",
          });
          this.setState({ StartDate: null });
          this.setState({ EndDate: null });
          this.setState({ datePickerIsOpen: false });
          this.setState({ PopUpBit: true });
        }
      });
    }
  };
  openDatePicker = () => {
    if (this.state.datePickerIsOpen == false) {
      this.setState({
        datePickerIsOpen: true,
      });
    } else {
      this.setState({
        datePickerIsOpen: false,
      });
    }
  };
  _onSelect = (e) => {
    this.setState({ ShowProjectList: false });
    var _projectName = e.target.value;
    this.setState({ ShowProjectName: _projectName });
    this.setState({ SearchProject: "" });
    // this.setState({ lstSearchProject: null }, () => {
    //   this.LoadProjectGrid();
    // });
    const index = e.target.selectedIndex;
    const el = e.target.childNodes[index];
    var _projectID = el.getAttribute("ddAttrProjectID");
    if (_projectID !== null) {
      this.setState({ lstSearchProject: null });
      this.setState({ SearchProject: "" });
      this.setState(
        {
          ProjectId: _projectID,
        },
        () => {
          if (_projectID == "-1") {
            this.setState({
              lstProjectData: this.state.lstProjectDataAll,
            });
          } else {
            var CheckProject = this.state.lstProjectDataAll.filter(
              (SName) => SName.ProjectID == _projectID
            );
            this.setState({
              lstProjectData: CheckProject,
            });
          }

          // this.LoadProjectGrid();
        }
      );
    }
  };
  _onSearchSelect = (e) => {
    this.setState({ ShowProjectName: "All Project" });
    this.setState({ ProjectId: -1 });
    var _projectID = e.target.getAttribute("ddAttrProjectID");
    var _projectName = e.target.getAttribute("ddAttrProjectName");
    if (_projectID !== null) {
      const temobj = [
        {
          ProjectID: _projectID,
          ProjectName: _projectName,
        },
      ];

      this.setState({ SearchProject: _projectName });
      this.setState(
        {
          lstSearchProject: temobj,
        },
        () => {
          this.LoadProjectGrid();
        }
      );
    }
  };
  CloseModal = () => {
    if (this.state.Title == "SUCCESS") {
      this.setState({
        AddProjectField: {
          ProjectName: "",
          StartDate: null,
          EndDate: null,
          Status: "",
          TaskOwner: "",
          ProjectType: "",
          Status: "",
          ProjectDescription: "",
          Client: null,
        },
      });
      this.setState({ currentBudgetYear: new Date() });
      this.setState({
        monthBudget: {
          January: 0,
          February: 0,
          March: 0,
          April: 0,
          May: 0,
          June: 0,
          July: 0,
          August: 0,
          September: 0,
          October: 0,
          November: 0,
          December: 0,
        },
      });
      if (this.state.AddProjectPopBit == true) {
        this.setState({ StartDate: null, EndDate: null, ProjectId: -1 }, () => {
          this.LoadProjectGrid();
          /* this.LoadProjectName(); */
          this.LoadProjectNameLazyLoading();
        });
      }
      this.LoadProjectGrid(); //for update refresh
      localStorage.setItem("ProjectImg", null);

      this.setState({ AddProjectPopBit: false });
      this.setState({ UpdateProjectPopBit: false });
    }
    this.setState({ PopUpBit: false });
  };
  CloseAddProjectModal = () => {
    this.setState({
      AddProjectField: {
        ProjectName: "",
        StartDate: null,
        EndDate: null,
        TaskOwner: "",
        ProjectType: "",
        Status: "",
        ProjectDescription: "",
        Client: null,
      },
    });
    this.setState({
      monthBudget: {
        January: 0,
        February: 0,
        March: 0,
        April: 0,
        May: 0,
        June: 0,
        July: 0,
        August: 0,
        September: 0,
        October: 0,
        November: 0,
        December: 0,
      },
    });
    this.setState({ currentBudgetYear: new Date() });
    this.setState({ errorStyle: {} });
    this.setState({ AddProjectPopBit: false });

    localStorage.setItem("ProjectImg", null);
  };
  CloseUpdateProjectModal = () => {
    this.setState({
      AddProjectField: {
        ProjectName: "",
        StartDate: null,
        EndDate: null,
        TaskOwner: "",
        ProjectType: "",
        Status: "",
        ProjectDescription: "",
        Client: null,
      },
    });
    this.setState({
      monthBudget: {
        January: 0,
        February: 0,
        March: 0,
        April: 0,
        May: 0,
        June: 0,
        July: 0,
        August: 0,
        September: 0,
        October: 0,
        November: 0,
        December: 0,
      },
    });
    this.setState({ currentBudgetYear: new Date() });
    this.setState({ errorStyle: {} });
    this.setState({ UpdateProjectPopBit: false });
    localStorage.setItem("ProjectImg", null);
  };
  errorStyleRemovedAddProject = () => {
    this.setState({ errorStyle: {} });
  };
  CloseImageModal = () => {
    this.setState({ ImageModalPopBit: false });
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  ChangeGrid = () => {
    if (this.state.iconChange == "fa fa-th-list") {
      this.setState({ iconChange: "fa fa-th-large" });
    } else {
      this.setState({ iconChange: "fa fa-th-list" });
    }
  };
  OpenAddProjectModal = () => {
    this.setState({
      AddProjectPopBit: true,
      AddProjectField: {
        ...this.state.AddProjectField,
        Status: "Active",
      },
    });
  };
  OpenUpdateProjectModal = () => {
    this.setState({ UpdateProjectPopBit: true }, () => {
      this.lstProjectBudget();
    });
  };
  OpenImageModal = () => {
    this.setState({ ImageModalPopBit: true });
  };
  incrementYear = () => {
    this.setState(
      {
        currentBudgetYear: moment(this.state.currentBudgetYear).add(1, "years"),
      },
      () => {
        if (this.state.UpdateProjectPopBit == true) {
          this.lstProjectBudget();
        }
      }
    );
  };
  decrementYear = () => {
    this.setState(
      {
        currentBudgetYear: moment(this.state.currentBudgetYear).subtract(
          1,
          "years"
        ),
      },
      () => {
        if (this.state.UpdateProjectPopBit == true) {
          this.lstProjectBudget();
        }
      }
    );
  };
  addProject = () => {
    try {
      var currentBudgetYear = moment(this.state.currentBudgetYear).format(
        "MM/DD/YYYY"
      );
      var StartDate = moment(this.state.AddProjectField.StartDate).format(
        "MM/DD/YYYY"
      );
      var EndDate = moment(this.state.AddProjectField.EndDate).format(
        "MM/DD/YYYY"
      );
      const userObj = {
        ID: -1,
        Name: this.state.AddProjectField.ProjectName,
        Type: this.state.AddProjectField.ProjectType,
        EstimatedHours: 0,
        StartDate: StartDate,
        EndDate: EndDate,
        ProjectOwner: this.state.AddProjectField.TaskOwner,
        ProjectDescription: this.state.AddProjectField.ProjectDescription,
        IsActive: this.state.AddProjectField.Status === "Active" ? 1 : 0,
        isUpdateProjectBudget: 0,
        ProjectBudgetYear: currentBudgetYear,
        ContractHours: 0,
        JanContract: this.state.monthBudget.January,
        FebContract: this.state.monthBudget.February,
        MarContract: this.state.monthBudget.March,
        AprContract: this.state.monthBudget.April,
        MayContract: this.state.monthBudget.May,
        JunContract: this.state.monthBudget.June,
        JulContract: this.state.monthBudget.July,
        AugContract: this.state.monthBudget.August,
        SepContract: this.state.monthBudget.September,
        OctContract: this.state.monthBudget.October,
        NovContract: this.state.monthBudget.November,
        DecContract: this.state.monthBudget.December,
        ClientID: this.state.AddProjectField.Client,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/InsertProject`,
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
        if (res.data.StatusCode == 200) {
          this.projectimgUpoad();

          this.setState({ Title: "SUCCESS" });
          this.setState({ errorMsg: "Project Added Successfully" });
          this.setState({ PopUpBit: true });
        } else {
          this.setState({ Title: "Failed" });
          this.setState({ errorMsg: "Process Failed" });
          this.setState({ PopUpBit: true });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  updateProject = () => {
    try {
      var currentBudgetYear = moment(this.state.currentBudgetYear).format(
        "MM/DD/YYYY"
      );
      var StartDate = moment(this.state.AddProjectField.StartDate).format(
        "MM/DD/YYYY"
      );
      var EndDate = moment(this.state.AddProjectField.EndDate).format(
        "MM/DD/YYYY"
      );
      var IsActive =
        this.state.AddProjectField.Status === true
          ? 1
          : this.state.AddProjectField.Status === false
          ? 0
          : null;
      if (IsActive === null) {
        IsActive =
          this.state.AddProjectField.Status === "1"
            ? 1
            : this.state.AddProjectField.Status === "0"
            ? 0
            : null;
      }
      const userObj = {
        ID: this.state.AddProjectField.ProjectID,
        Name: this.state.AddProjectField.ProjectName,
        Type: this.state.AddProjectField.ProjectType,
        EstimatedHours: 0,
        StartDate: StartDate,
        EndDate: EndDate,
        ProjectOwner: this.state.AddProjectField.TaskOwner,
        ProjectDescription: this.state.AddProjectField.ProjectDescription,
        IsActive: IsActive,
        isUpdateProjectBudget: 1,
        ProjectBudgetYear: currentBudgetYear,
        ContractHours: 0,
        JanContract: this.state.monthBudget.January,
        FebContract: this.state.monthBudget.February,
        MarContract: this.state.monthBudget.March,
        AprContract: this.state.monthBudget.April,
        MayContract: this.state.monthBudget.May,
        JunContract: this.state.monthBudget.June,
        JulContract: this.state.monthBudget.July,
        AugContract: this.state.monthBudget.August,
        SepContract: this.state.monthBudget.September,
        OctContract: this.state.monthBudget.October,
        NovContract: this.state.monthBudget.November,
        DecContract: this.state.monthBudget.December,
        ClientID: this.state.AddProjectField.ClientID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/InsertProject`,
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
        if (res.data.StatusCode == 200) {
          this.projectimgUpoad();
          this.setState({ Title: "SUCCESS" });
          this.setState({ errorMsg: "Project Update Successfully" });
          this.setState({ PopUpBit: true });
        } else {
          this.setState({ Title: "Failed" });
          this.setState({ errorMsg: "Process Failed" });
          this.setState({ PopUpBit: true });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  lstProjectBudget = () => {
    try {
      var currentBudgetYear = moment(this.state.currentBudgetYear).format(
        "MM/DD/YYYY"
      );
      const projectBudgetDetailObj = {
        ID: this.state.AddProjectField.ProjectID,
        ProjectBudgetYear: currentBudgetYear,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsBudgetDetail`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: projectBudgetDetailObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState({ lstProjectBudgetDetail: res.data.Result });
          this.setState({
            monthBudget: {
              January: res.data.Result[0].JanContract,
              February: res.data.Result[0].FebContract,
              March: res.data.Result[0].MarContract,
              April: res.data.Result[0].AprContract,
              May: res.data.Result[0].MayContract,
              June: res.data.Result[0].JunContract,
              July: res.data.Result[0].JulContract,
              August: res.data.Result[0].AugContract,
              September: res.data.Result[0].SepContract,
              October: res.data.Result[0].OctContract,
              November: res.data.Result[0].NovContract,
              December: res.data.Result[0].DecContract,
            },
          });
        } else {
          this.setState({
            monthBudget: {
              January: 0,
              February: 0,
              March: 0,
              April: 0,
              May: 0,
              June: 0,
              July: 0,
              August: 0,
              September: 0,
              October: 0,
              November: 0,
              December: 0,
            },
          });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  projectimgUpoad = () => {
    try {
      if (localStorage.getItem("ProjectImg") != "null") {
        var imgSrc = localStorage.getItem("ProjectImg");
        var base64string = imgSrc;
        var xhr = new XMLHttpRequest(); //convert that blob to base64
        xhr.responseType = "blob";
        xhr.onload = async function () {
          var recoveredBlob = xhr.response;
          var reader = new FileReader();
          reader.onload = async function () {
            var blobAsDataUrl = reader.result;
            await this.setState({ base64Image: blobAsDataUrl }, () => {
              var base64 = this.state.base64Image;
              var index = base64.indexOf(",");
              var UserImgFile = base64.substr(index + 1);
              var Imagefileobj = { UserId: 0, Imagefileobj: null };
              if (this.state.AddProjectPopBit == true) {
                Imagefileobj = {
                  ProjectID: 0, //HANDLING IT IN BACKEND
                  Imagefileobj: UserImgFile,
                };
              } else {
                Imagefileobj = {
                  ProjectID: this.state.AddProjectField.ProjectID,
                  Imagefileobj: UserImgFile,
                };
              }
              axios({
                method: "post",
                url: `${process.env.REACT_APP_BASE_URL}Projects/ProjectPic`,
                headers: {
                  Authorization:
                    "Bearer " + localStorage.getItem("access_token"),
                  encrypted: localStorage.getItem("EncryptedType"),
                  Role_Type: Cookies.get("Role"),
                },
                data: Imagefileobj,
              }).then((res) => {
                if (res.data?.StatusCode === 401) {
                  this.logout();
                }
                /*   this.setState({ Title: "Success" });
                this.setState({ errorMsg: res.data.Result });
                this.setState({ PopUpBitImg: true });
                console.log(res); */
              });
            });
          }.bind(this);
          var test = reader.readAsDataURL(recoveredBlob);
        }.bind(this);
        xhr.open("GET", base64string);
        xhr.send();
      } else {
        this.setState({ Title: "Error" });
        this.setState({
          errorMsg: "Please Select an Image",
        });
        this.setState({ PopUpBitImg: true });
      }
    } catch (e) {
      console.log(e);
    }
  };
  SearchProject = () => {
    try {
      const SearchObj = {
        ProjectName: this.state.SearchProject,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/SearchProject`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: SearchObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState({ lstSearchProject: res.data.Result });
        }
      });
    } catch (e) {
      console.log(e);
    }
  };
  SearchFilterValue = () => {
    this.setState(
      {
        SearchProjectID: this.state.ProjectId,
        SearchFilterStatus: this.state.FilterStatus,
      },
      () => {}
    );
  };
  WithoutSearchFilterValue = () => {
    this.setState({
      ProjectId: this.state.SearchProjectID,
      FilterStatus: this.state.SearchFilterStatus,
    });
  };
  SearchProjectGrid = () => {
    this.setState({
      Loading: true,
    });
    var getprojectlist = [];
    var filter_status =
      this.state.FilterStatus == "true"
        ? true
        : this.state.FilterStatus == "false"
        ? false
        : null;
    if (this.state.ProjectId != -1 && filter_status == null) {
      getprojectlist = this.state.lstProjectDataAll.filter(
        (SName) => SName.ProjectID == this.state.ProjectId
      );
    }
    if (this.state.ProjectId == -1 && filter_status != null) {
      getprojectlist = this.state.lstProjectDataAll.filter(
        (SName) => SName.isActive == filter_status
      );
    }
    if (this.state.ProjectId != -1 && filter_status != null) {
      getprojectlist = this.state.lstProjectDataAll.filter(
        (SName) =>
          SName.ProjectID == this.state.ProjectId &&
          SName.isActive == filter_status
      );
    }
    this.setState({
      lstProjectData: getprojectlist,
      Filtermodal: false,
      Loading: false,
    });
  };
  render() {
    var ProjectNameList = null;
    ProjectNameList =
      this.state.lstProjectDataAll != null
        ? this.state.lstProjectDataAll?.map((item, key) => {
            //
            return (
              <option
                // onClick={this._onSelect}
                ddAttrProjectID={item.ProjectID}
                ddAttr={item.ProjectDescription}
                value={item.ProjectName}
              >
                {item.ProjectName}
              </option>
            );
          })
        : "";

    var SearchProjectNameList = null;
    SearchProjectNameList =
      this.state.lstSearchProject !== null
        ? this.state.lstSearchProject.map((item) => {
            return (
              <li
                onClick={this._onSearchSelect}
                ddAttrProjectID={item.ProjectID}
                ddAttrProjectName={item.ProjectName}
              >
                {item.ProjectName}
              </li>
            );
          })
        : "";

    return (
      <>
        <div className="content mt-3" id="project_content_id">
          <div className="page-heading pb-1" style={{ marginBottom: "20px" }}>
            <div className="row">
              <div className="col-sm-4">
                <h2 className="m_font_heading">Manage Projects</h2>
              </div>

              <div
                class="col-sm-8 text-right pr-0 custom_select_latest_outer"
                id="ems_manage_all_pro"
              >
                <ButtonGroup size="sm" className="mb-2" id="diff_btn_data">
                  <div
                    className="mt-1 thumb_list"
                    style={{ marginRight: "10px" }}
                  >
                    <span className="btn_grid_change" onClick={this.ChangeGrid}>
                      <i class={this.state.iconChange} aria-hidden="true"></i>
                    </span>
                  </div>

                  {/* <Dropdown
                  className="mr-1 ml-2"
                  id="project_modules"
                    style={{ maxWidth: "200px", float: "right" }}
                    menuAlign="left"
                  >
                    <Dropdown.Toggle
                      id="dropdown-basic"
                      className="btn-black mr-1 project_list_dropdwon"
                    >
                      {this.state.ShowProjectName}
                    </Dropdown.Toggle>

                    <Dropdown.Menu className="align_top dashboard_dropdown">
                      <Dropdown.Item>
                        <div
                          className="custom_select_latest"
                          id="scrollableDiv"
                        >
                          <InfiniteScroll
                            dataLength={ProjectNameList.length} //This is important field to render the next data
                            next={() => {
                              this.setState(
                                {
                                  ProjectLazyLoading: {
                                    ...this.state.ProjectLazyLoading,
                                    ProjectRecoardPerPage:
                                      this.state.ProjectLazyLoading
                                        .ProjectRecoardPerPage + 20,
                                    CurrentProjectDataCount:
                                      ProjectNameList.length,
                                  },
                                },
                                () => {
                                  if (
                                    this.state.ProjectLazyLoading
                                      .CurrentProjectDataCount +
                                      20 ==
                                    this.state.ProjectLazyLoading
                                      .ProjectRecoardPerPage
                                  ) {
                                    this.LoadProjectNameLazyLoading();
                                  } else {
                                    this.setState(
                                      {
                                        ProjectLazyLoading: {
                                          ...this.state.ProjectLazyLoading,
                                          ProjectHasMore: false,
                                        },
                                      },
                                      this.LoadProjectNameLazyLoading()
                                    );
                                  }
                                }
                              );
                            }}
                            hasMore={
                              this.state.ProjectLazyLoading.ProjectHasMore
                            }
                            loader={
                              <p className="text-center mt-3">
                                <i class="fa fa-spinner fa-pulse"></i> Loading
                                ...
                              </p>
                            }
                            scrollableTarget="scrollableDiv"
                            endMessage={
                              <p
                                style={{
                                  textAlign: "center",
                                  lineHeight: "40px",
                                }}
                              >
                                <b>Yay! You have seen it all</b>
                              </p>
                            }
                          >
                            <option
                              ddAttrProjectID="-1"
                              value="All Project"
                              onClick={this._onSelect}
                            >
                              All Project
                            </option>
                            {ProjectNameList}
                          </InfiniteScroll>
                        </div>
                      </Dropdown.Item>
                    </Dropdown.Menu>
                  </Dropdown> */}

                  <div
                    className="input-group dashboard-input-group date project_all_set"
                    id="ems_select_pro_data"
                  >
                    <select
                      className="input-group-addon"
                      onChange={this._onSelect}
                    >
                      <option
                        className="options_value"
                        ddAttrProjectID="-1"
                        value="All Project"
                      >
                        All Project
                      </option>
                      {ProjectNameList}
                    </select>
                  </div>

                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <button
                      onClick={this.OpenAddProjectModal}
                      type="button"
                      className="btn-black mr-2 ml-2"
                      data-toggle="modal"
                      data-target="#reportsFilter"
                    >
                      Add Project <i className="menu-icon fa fa-plus"></i>
                    </button>
                  ) : (
                    ""
                  )}

                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <button
                      onClick={() => {
                        this.setState({
                          Filtermodal: true,
                        });
                      }}
                      type="button"
                      className="btn-black mr-2"
                      data-toggle="modal"
                      data-target="#reportsFilter"
                    >
                      Filter
                    </button>
                  ) : (
                    ""
                  )}

                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <button
                      onClick={() => {
                        this.setState({
                          lstProjectData: this.state.lstProjectDataAll,
                          ProjectId: -1,
                          FilterStatus: null,
                        });
                      }}
                      type="button"
                      className="btn-black"
                      data-toggle="modal"
                      data-target="#reportsFilter"
                    >
                      Clear Filter
                    </button>
                  ) : (
                    ""
                  )}
                </ButtonGroup>
              </div>
            </div>
          </div>

          <div
            className="row"
            style={
              this.state.iconChange == "fa fa-th-large"
                ? { display: "none" }
                : { display: "block" }
            }
          >
            <div className="col-sm-12 pr-0">
              <div className="card">
                <div className="card-header">
                  <div className="row align-items-center">
                    <div className="col-sm-6">
                      <h3>Projects</h3>
                    </div>
                    <div className="col-sm-6 text-right">
                      <h6>
                        <span
                          className={`HoursDays ${this.state.DayHourToggle.dayStyle}`}
                          onClick={() => {
                            this.setState({
                              DayHourToggle: {
                                dayStyle: "btn_underline",
                                hourStyle: "",
                              },
                            });
                            this.setState({ dataFormat: "Days" });
                          }}
                        >
                          Days
                        </span>
                        <span
                          className={`HoursDays ${this.state.DayHourToggle.hourStyle}`}
                          style={{ marginLeft: "10px" }}
                          onClick={() => {
                            this.setState({
                              DayHourToggle: {
                                dayStyle: "",
                                hourStyle: "btn_underline",
                              },
                            });
                            this.setState({ dataFormat: "Hours" });
                          }}
                        >
                          Hours
                        </span>
                      </h6>
                    </div>
                  </div>
                </div>
                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div className="card-body">
                    <div className="table-responsive easing_row">
                      <TableContainer component={"div"}>
                        <Table id="bootstrap-data-table" className="table">
                          <thead>
                            <tr>
                              <th scope="col">Project Name</th>
                              <th scope="col">
                                {this.state.dataFormat == "Days"
                                  ? "Completed (Days)"
                                  : this.state.dataFormat == "Hours"
                                  ? "Completed (Hours)"
                                  : "Completed (Days)"}
                              </th>

                              <th scope="col">
                                {this.state.dataFormat == "Days"
                                  ? "Budgeted (Days)"
                                  : this.state.dataFormat == "Hours"
                                  ? "Budgeted (Hours)"
                                  : "Budgeted (Days)"}
                              </th>
                              <th scope="col">Progress Bar</th>
                              <th scope="col">Progress Status</th>
                              <th scope="col">Status</th>
                              <th scope="col"></th>
                            </tr>
                          </thead>
                          <tbody>
                            {this.state.lstProjectData != null
                              ? this.state.lstProjectData
                                  .slice(
                                    this.state.page * this.state.rowsPerPage,
                                    this.state.page * this.state.rowsPerPage +
                                      this.state.rowsPerPage
                                  )
                                  .map((data, index) => {
                                    var progressbarStyle = "";
                                    data.CompletedPercentage <= 100
                                      ? (progressbarStyle =
                                          "progress-bar bg-gradient")
                                      : (progressbarStyle =
                                          "progress-bar bg-danger");
                                    return (
                                      <tr className="tr_effect">
                                        <td
                                          scope="row"
                                          style={{
                                            paddingRight: 0,
                                            width: "350px",
                                          }}
                                        >
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            {/* {index + 1}.  */}
                                            {data.ProjectName}
                                          </Link>
                                        </td>
                                        <td className="ems_cmp_day">
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            {this.state.dataFormat == "Days"
                                              ? data.TotalDaysSpend
                                              : this.state.dataFormat == "Hours"
                                              ? parseFloat(
                                                  data.TotalDaysSpend * 8
                                                ).toFixed(2)
                                              : data.TotalDaysSpend}
                                          </Link>
                                        </td>
                                        <td className="ems_mng_prj_budg">
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            {this.state.dataFormat == "Days"
                                              ? data.TotalEstimatedDays
                                              : this.state.dataFormat == "Hours"
                                              ? parseFloat(
                                                  data.TotalEstimatedDays * 8
                                                ).toFixed(2)
                                              : data.TotalEstimatedDays}
                                          </Link>
                                        </td>
                                        <td style={{ width: "300px" }}>
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            <Tooltip
                                              title="Progress of the Project"
                                              arrow
                                            >
                                              <div
                                                className="progress"
                                                style={{
                                                  position: "relative",
                                                  boxShadow:
                                                    " 3px 4px 10px #888888",
                                                }}
                                              >
                                                <div
                                                  className={
                                                    data.Total_Hours_Spent ==
                                                    "0.00"
                                                      ? null
                                                      : "progress-bar bg-gradient"
                                                  }
                                                  role="progressbar"
                                                  style={{
                                                    width: `100%`,
                                                    textAlign: "center",
                                                  }}
                                                  aria-valuenow={`${data.Total_Hours_Spent}`}
                                                  aria-valuemin="0"
                                                  aria-valuemax="100"
                                                >
                                                  <span
                                                    style={{
                                                      zIndex: "1",
                                                      color: "black",
                                                    }}
                                                  >
                                                    {this.state.dataFormat ==
                                                    "Days"
                                                      ? data.TotalDaysSpend +
                                                        " Days"
                                                      : this.state.dataFormat ==
                                                        "Hours"
                                                      ? parseFloat(
                                                          data.TotalDaysSpend *
                                                            8
                                                        ).toFixed(2) + " hrs"
                                                      : data.TotalDaysSpend +
                                                        " Days"}
                                                  </span>
                                                </div>
                                              </div>
                                            </Tooltip>
                                          </Link>
                                        </td>

                                        <td>
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            {data.IsLimitExceed == false
                                              ? "In-Progress"
                                              : data.CompletedPercentage == 100
                                              ? "Completed"
                                              : "Budget Exceeded"}
                                          </Link>
                                        </td>
                                        <td>
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            {data.isActive == 1
                                              ? "Active"
                                              : "In-Active"}
                                          </Link>
                                        </td>
                                        {Cookies.get("Role") ===
                                        "SuperAdmin" ? (
                                          <td>
                                            <i
                                              class="fa fa-pencil-square-o"
                                              aria-hidden="true"
                                              style={{
                                                cursor: "pointer",
                                                fontSize: "18px",
                                              }}
                                              onClick={() => {
                                                this.setState(
                                                  {
                                                    AddProjectField: {
                                                      ProjectID: data.ProjectID,
                                                      ProjectName:
                                                        data.ProjectName,
                                                      StartDate: new Date(
                                                        data.StartDate
                                                      ),
                                                      EndDate: new Date(
                                                        data.EndDate
                                                      ),
                                                      TaskOwner:
                                                        data.ProjectOwner,
                                                      ProjectType:
                                                        data.ProjectType,
                                                      Status: data.isActive,
                                                      ProjectDescription:
                                                        data.ProjectDescription,
                                                      ClientID: data.ClientID,
                                                    },
                                                  },
                                                  () => {
                                                    this.OpenUpdateProjectModal();
                                                  }
                                                );
                                              }}
                                            ></i>
                                          </td>
                                        ) : (
                                          ""
                                        )}
                                      </tr>
                                    );
                                  })
                              : null}
                          </tbody>
                        </Table>
                      </TableContainer>
                      <TablePagination
                        component="div"
                        count={this.state.lstProjectDataCount}
                        page={this.state.page}
                        onChangePage={this.handleChangePage}
                        rowsPerPage={this.state.rowsPerPage}
                        onChangeRowsPerPage={this.handleChangeRowsPerPage}
                      />
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>

          <div
            style={
              this.state.iconChange == "fa fa-th-list"
                ? { display: "none" }
                : { display: "block" }
            }
          >
            <div class="row grid_view" id="ems_project_calendar_desk_view">
              {this.state.lstProjectData != null
                ? this.state.lstProjectData
                    .slice(
                      this.state.page * this.state.rowsPerPage,
                      this.state.page * this.state.rowsPerPage +
                        this.state.rowsPerPage
                    )
                    .map((data, index) => {
                      var progressbarStyle = "";
                      data.CompletedPercentage <= 100
                        ? (progressbarStyle = "progress-bar bg-gradient")
                        : (progressbarStyle = "progress-bar bg-danger");
                      return (
                        <div class="col-sm-6">
                          <div class="card">
                            <div class="card-body">
                              <div
                                class="row"
                                style={{ marginBottom: "-20px" }}
                              >
                                <div class="col-lg-2 col-sm-3 col-3">
                                  <div class="profile-placeholder project_pic">
                                    <div class="circle">
                                      <img
                                        src={`http://rezaidems-001-site5.dtempurl.com/ProjectImages/ProjectImgID${
                                          data?.ProjectID
                                        }.png?cache=${new Date()}`}
                                        onError={(e) => {
                                          e.target.onerror = null;
                                          e.target.src = NoPic;
                                        }}
                                        alt=""
                                      />
                                      {/* <img
                                        class="profile-pic"
                                        src="images/profile-avator.png"
                                        alt=""
                                      /> */}
                                    </div>
                                  </div>
                                </div>
                                <div class="col-lg-9 col-md-9 col-sm-9 col-8">
                                  <h4 class="mt-3">{data.ProjectName}</h4>

                                  <div class="dropdown for-message project_listing_dropdown">
                                    <Dropdown
                                      menuAlign="right"
                                      onSelect={(evt) => {
                                        if (evt == "Edit") {
                                          this.setState(
                                            {
                                              AddProjectField: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: new Date(
                                                  data.StartDate
                                                ),
                                                EndDate: new Date(data.EndDate),
                                                TaskOwner: data.ProjectOwner,
                                                ProjectType: data.ProjectType,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Status: data.IsActive,
                                                ClientID: data.ClientID,
                                              },
                                            },
                                            () => {
                                              this.OpenUpdateProjectModal();
                                            }
                                          );
                                        }
                                      }}
                                    >
                                      <Dropdown.Toggle
                                        variant="default"
                                        id="dropdown-basic"
                                        style={{
                                          backgroundColor: "transparent",
                                          color: "#000",
                                        }}
                                      >
                                        <span
                                          id="porjectsAction"
                                          data-toggle="dropdown"
                                          aria-haspopup="true"
                                          aria-expanded="false"
                                        >
                                          <i class="ti-more-alt ico-icon cursor-pointer"></i>
                                        </span>
                                      </Dropdown.Toggle>

                                      <Dropdown.Menu className="align_top">
                                        <Dropdown.Item>
                                          <Link
                                            to={{
                                              pathname: "/ProjectDetails",
                                              ProjectData: {
                                                ProjectID: data.ProjectID,
                                                ProjectName: data.ProjectName,
                                                StartDate: data.StartDate,
                                                EndData: data.EndDate,
                                                ProjectType: data.ProjectType,
                                                ProjectOwner: data.ProjectOwner,
                                                ProjectDescription:
                                                  data.ProjectDescription,
                                                Progress:
                                                  data.CompletedPercentage,
                                                Status: data.isActive,
                                                ClientID: data.ClientID,
                                              },
                                            }}
                                          >
                                            Detail
                                          </Link>
                                        </Dropdown.Item>
                                        <Dropdown.Item eventKey="Edit">
                                          Edit
                                        </Dropdown.Item>
                                      </Dropdown.Menu>
                                    </Dropdown>
                                  </div>
                                </div>
                              </div>
                              <table class="table border-0 table-responsive">
                                <thead>
                                  <tr>
                                    <th>Completed Days</th>
                                    <th>Budgeted Days</th>
                                    <th>Progress Status</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  <tr>
                                    <Tablestyle>
                                      {data.TotalDaysSpend}
                                    </Tablestyle>
                                    <Tablestyle>
                                      {data.TotalEstimatedDays}
                                    </Tablestyle>
                                    <Tablestyle>
                                      {data.IsLimitExceed == false
                                        ? "InProgress"
                                        : data.CompletedPercentage == 100
                                        ? "Completed"
                                        : "Budget Exceeded"}
                                    </Tablestyle>
                                  </tr>
                                </tbody>
                              </table>
                              <hr
                                style={{
                                  marginBottom: "0.5rem",
                                  marginTop: "0.5rem",
                                  borderTop: "2px solid rgba(0,0,0,.1)",
                                }}
                              />
                              <h6 style={{ padding: "0 0 15px 10px" }}>
                                Progress Bar
                              </h6>

                              <div
                                class="progress"
                                style={{
                                  position: "relative",
                                  boxShadow: " 3px 4px 10px #888888",
                                  justifyContent: "center",
                                }}
                              >
                                <span
                                  style={{
                                    position: "absolute",
                                    top: "-1px",
                                    zIndex: "1",
                                    color: "black",
                                  }}
                                >
                                  {data.Total_Hours_Spent}hrs
                                </span>
                                <div
                                  className={
                                    data.Total_Hours_Spent == "0.00"
                                      ? null
                                      : "rogress-bar bg-gradient"
                                  }
                                  role="progressbar"
                                  style={{
                                    width: `100%`,
                                  }}
                                  aria-valuenow={`${data.Total_Hours_Spent}`}
                                  aria-valuemin="0"
                                  aria-valuemax="100"
                                ></div>
                              </div>
                            </div>
                          </div>
                        </div>
                      );
                    })
                : null}

              <TablePagination
                component="div"
                count={this.state.lstProjectDataCount}
                page={this.state.page}
                onChangePage={this.handleChangePage}
                rowsPerPage={this.state.rowsPerPage}
                onChangeRowsPerPage={this.handleChangeRowsPerPage}
              />
            </div>
          </div>
        </div>
        <AddProjectPopUp
          AddProjectShow={this.state.AddProjectPopBit}
          onHide={this.CloseAddProjectModal}
          ShowImageModal={this.state.ImageModalPopBit}
          imageOnShow={this.OpenImageModal}
          imageOnHide={this.CloseImageModal}
          MonthArray={this.state.monthArray}
          HandleAddProject={this.handleAddProject}
          HandleStartDateChange={this.handleStartDateChange}
          ShowStartDate={this.state.AddProjectField.StartDate}
          HandleEndDateChange={this.handleEndDateChange}
          ShowEndDate={this.state.AddProjectField.EndDate}
          HandleMonthBudgetProject={this.handleMonthBudgetProject}
          MonthBudgetState={this.state.monthBudget}
          CheckValidateAddProject={this.CheckValidationAddProject}
          //FieldErrorStyle={this.state.errorStyle}
          ErrorStyleRemovedAddProject={this.errorStyleRemovedAddProject}
          CurrentBudgetYear={this.state.currentBudgetYear}
          IncrementYear={this.incrementYear}
          DecrementYear={this.decrementYear}
          errorStyle={this.state.errorStyle}
          HandelErrorRemove={this.HandelErrorRemove}
          AddStatus={this.state.AddProjectField.Status}
        />

        <UpdateProjectPopUp
          AddProjectShow={this.state.UpdateProjectPopBit}
          onHide={this.CloseUpdateProjectModal}
          ShowImageModal={this.state.ImageModalPopBit}
          imageOnShow={this.OpenImageModal}
          imageOnHide={this.CloseImageModal}
          MonthArray={this.state.monthArray}
          HandleAddProject={this.handleAddProject}
          HandleStartDateChange={this.handleStartDateChange}
          HandleEndDateChange={this.handleEndDateChange}
          HandleMonthBudgetProject={this.handleMonthBudgetProject}
          MonthBudgetState={this.state.monthBudget}
          CheckValidateUpdateProject={this.CheckValidationUpdateProject}
          errorStyle={this.state.errorStyle}
          ErrorStyleRemovedAddProject={this.errorStyleRemovedAddProject}
          CurrentBudgetYear={this.state.currentBudgetYear}
          IncrementYear={this.incrementYear}
          DecrementYear={this.decrementYear}
          ShowData={this.state.AddProjectField}
          HandelErrorRemove={this.HandelErrorRemove}
        />
        <SureMsgPopUp
          Title={this.state.Title}
          ShowMsgs={this.state.errorMsg}
          show={this.state.ShowMsgPopUp}
          onConfrim={this.CheckPopUp}
          onHide={this.CloseModal1}
        />
        <PopUpModal
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.PopUpBit}
          onHide={this.CloseModal}
        />
        {this.state.Filtermodal == true ? (
          <FilterModal
            Title={this.state.Title}
            errorMsgs={this.state.errorMsg}
            show={this.state.Filtermodal}
            ProjectList={this.state.lstProjectDataAll}
            onHide={() => {
              this.setState(
                {
                  Filtermodal: false,
                },
                () => {
                  this.WithoutSearchFilterValue();
                }
              );
            }}
            changeprojectState={(e) => {
              this.setState({
                ProjectId: e.target.value,
              });
            }}
            ProjectId={this.state.ProjectId}
            FilterStatus={this.state.FilterStatus}
            ChangedStatus={(e) => {
              this.setState({
                FilterStatus: e.target.value,
              });
            }}
            onSubmit={this.SearchProjectGrid}
          />
        ) : null}
      </>
    );
  }
}

export default Projects;
