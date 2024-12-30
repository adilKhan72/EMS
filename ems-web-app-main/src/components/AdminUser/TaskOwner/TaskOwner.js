import React, { Component } from "react";
import axios from "axios";
import DatePicker from "react-datepicker";
import moment from "moment";
import "react-datepicker/dist/react-datepicker.css";
import "./TaskOwner.css";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import AddTaskOwnersPopup from "./AddTaskOwnersPopup";
import TaskOwnerFilterPopup from "./TaskOwnerFilterPopup";
import "../../Material_UI_Inputs/Material_UI_Inputs.css";
import Switch from "@material-ui/core/Switch";
import SureMsgPopUp from "./SureMsgPopUp";
import ConfirmPopUp from "./ConfirmPopUp";
import ProjectsListModal from "./ProjectsListModal";
import Loader from "../../Loader/Loader";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "react-crypt-gsm";
class TaskOwner extends Component {
  constructor(props) {
    super(props);
    this.state = {
      page: 0,
      rowsPerPage: 10,
      Loading: true,
      AddTaskOwnersShow: false,
      UserProjectsShow: false,
      UserIdForProjects: 0,
      TaskOwnerFilterShow: false,
      lstUser: [],
      TaskOwnerHandle: {
        UserProfileID: 0,
        UserName: "",
        Name: "",
        Department: "",
        Designation: "",
        Status: "",
        Email: "",
        AccountType: "",
        JoiningDate: "",
      },
      TaskOwnerHandleFilter: {
        Name: "",
        Department: "",
        Designation: "",
        Status: "",
        Email: "",
        AccountType: "",
      },
      ShowJoiningDate: false,
      SwitchState: [],
      ModalTitle: null,
      error_class: {
        UserName: "",
        Name: "",
        Department: "",
        Designation: "",
        Email: "",
        AccountType: "",
        Status: "",
        JoiningDate: "",
      },
      confirmpopup: false,
      ToggleIndex: null,
      confirmpopupmsg: null,
      IsUpdated: false,
      lstDesignation: [],
      lstDepartment: [],
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
    // this.LoadTaskOwner();
    this.LoadUserProfile();
    this.LoadDesignations();
    this.LoadDepartments();
  }
  /*   LoadTaskOwner = () => {
    try {
      const taskOwnerobj = { objTOName: "" };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}TaskOwners/GetTaskOwners`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: taskOwnerobj,
      }).then((res) => {
        this.setState({ lstTaskOwner: res.data.Result }, () => {
          this.setState({
            TaskOwnerCount: Object.keys(this.state.lstTaskOwner).length,
          });
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  }; */
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
  ShowHideUserProject = (type) => {
    if (type === "show") {
      this.setState({ UserProjectsShow: true });
      // this.CloseModal();
    } else {
      this.setState({ UserProjectsShow: false });
    }
  };

  LoadUserProfile = () => {
    this.setState({
      AddTaskOwnersShow: false,
      UserProjectsShow: false,
      TaskOwnerFilterShow: false,
    });
    try {
      if (!this.state.IsUpdated) {
        this.setState({
          TaskOwnerHandle: {
            UserProfileID: 0,
            UserName: "",
            Name: "",
            Department: "",
            Designation: "",
            Status: true,
            Email: "",
            AccountType: "",
            JoiningDate: "",
          },
          // TaskOwnerHandleFilter: {
          //   Name: "",
          //   Department: "",
          //   Designation: "",
          //   Status: "",
          //   Email: "",
          //   AccountType: "",
          // },
        });
      }
      this.setState({ Loading: true });
      const UserIdObj = {
        UserID: 0,
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
        var temp = [];

        this.setState({ lstUser: res.data.Result }, () => {
          this.state.lstUser.map((item, index) => {
            this.SetSwitchState(item.Status, temp);
          });
          this.setState({ SwitchState: temp }, () => {
            this.handleChangeSwitch();
            this.LoadFilterUserProfile();
          });
        });

        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadUserSearch = () => {
    this.setState({
      AddTaskOwnersShow: false,
      UserProjectsShow: false,
      TaskOwnerFilterShow: false,
    });
    try {
      this.setState({ Loading: true });
      const UserIdObj = {
        UserID: 0,
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
        var temp = [];
        this.setState({ lstUser: res.data.Result }, () => {
          this.state.lstUser.map((item, index) => {
            this.SetSwitchState(item.Status, temp);
          });
          this.setState({ SwitchState: temp }, () => {
            this.handleChangeSwitch();
          });
        });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  SearchFiltercheck = () => {
    if (
      this.state.TaskOwnerHandleFilter.Name !== "" ||
      this.state.TaskOwnerHandleFilter.Department !== "" ||
      this.state.TaskOwnerHandleFilter.Department !== "" ||
      this.state.TaskOwnerHandleFilter.Email !== "" ||
      this.state.TaskOwnerHandleFilter.AccountType !== "" ||
      this.state.TaskOwnerHandleFilter.Status !== true
    ) {
      this.LoadFilterUserProfile();
    } else {
      this.setState({ Title: "Filter Alert" });
      this.setState({
        errorMsg: "Please Select atleast one Filter!",
      });
      this.setState({ ShowMsgPopUp: true });
    }
  };
  LoadFilterUserProfile = () => {
    try {
      this.setState({ AddTaskOwnersShow: false });
      if (!this.state.IsUpdated) {
        this.setState({ page: 0 });
      }
      this.setState({ Loading: true });

      var IsActive =
        this.state.TaskOwnerHandleFilter.Status == "Active"
          ? 1
          : this.state.TaskOwnerHandleFilter.Status == "In-Active"
          ? 0
          : null;

      const FilterObj = {
        Name: this.state.TaskOwnerHandleFilter.Name,
        Department: this.state.TaskOwnerHandleFilter.Department,
        Designation: this.state.TaskOwnerHandleFilter.Designation,
        Status: IsActive,
        Email: this.state.TaskOwnerHandleFilter.Email,
        EMSRole: this.state.TaskOwnerHandleFilter.AccountType,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/GetFilterUserProfile`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: FilterObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        var temp = [];
        this.setState(
          { lstUser: res.data.Result, TaskOwnerFilterShow: false },
          () => {
            this.state.lstUser.map((item, index) => {
              this.SetSwitchState(item.Status, temp);
            });
            this.setState({ SwitchState: temp }, () => {
              this.handleChangeSwitch();
            });
          }
        );

        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  AddUpdateUserProfile = () => {
    try {
      var IsActive =
        this.state.TaskOwnerHandle.Status == true
          ? 1
          : this.state.TaskOwnerHandle.Status === false
          ? 0
          : null;
      if (IsActive === null) {
        IsActive =
          this.state.TaskOwnerHandle.Status == "1"
            ? 1
            : this.state.TaskOwnerHandle.Status == "0"
            ? 0
            : null;
      }
      const UserObj = {
        UserProfileID: this.state.TaskOwnerHandle.UserProfileID,
        UserName: this.state.TaskOwnerHandle.UserName,
        FullName: this.state.TaskOwnerHandle.Name,
        Department: this.state.TaskOwnerHandle.Department,
        Designation: this.state.TaskOwnerHandle.Designation,
        IsActive: IsActive,
        Email: this.state.TaskOwnerHandle.Email,
        AccountType: this.state.TaskOwnerHandle.AccountType,
        JoiningDate: this.state.TaskOwnerHandle.JoiningDate,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/InserUserProfile`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (this.state.TaskOwnerHandle.UserProfileID) {
          this.setState({ IsUpdated: true });
        }

        this.setState(
          {
            TaskOwnerHandle: {
              UserProfileID: 0,
              UserName: "",
              Name: "",
              Department: "",
              Designation: "",
              Status: "",
              Email: "",
              AccountType: "",
              JoiningDate: "",
            },
            ShowJoiningDate: false,
            SwitchState: [],
            SwitchStateAll: [],
          },
          () => {
            this.LoadFilterUserProfile();
          }
        );
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadDesignations = () => {
    try {
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Designation/GetUserDesignation`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstDesignation: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadDepartments = () => {
    try {
      axios({
        method: "Post",
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
        this.setState({ lstDepartment: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  SwitchUpdateCall = (IsActive) => {
    try {
      const UserObj = {
        UserProfileID: this.state.TaskOwnerHandle.UserProfileID,
        UserName: null,
        FullName: null,
        Department: null,
        Designation: null,
        IsActive: IsActive,
        Email: null,
        AccountType: null,
        JoiningDate: null,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/InserUserProfile`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.LoadUserProfile();
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  TaskOwnerHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;

      this.setState({
        TaskOwnerHandle: {
          ...this.state.TaskOwnerHandle,
          [name]: value,
        },
      });
    } catch (ex) {
      //alert(ex);
    }
  };

  TaskOwnerHandleChangeFilter = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;

      this.setState({
        TaskOwnerHandleFilter: {
          ...this.state.TaskOwnerHandleFilter,
          [name]: value,
        },
      });
    } catch (ex) {
      //alert(ex);
    }
  };
  JoiningDateHandler = (date) => {
    try {
      this.setState({ ShowJoiningDate: moment(date).format("DD/MM/YYYY") });
      var tempDate = moment(date).format("DD/MM/YYYY");
      this.setState({
        TaskOwnerHandle: {
          ...this.state.TaskOwnerHandle,
          JoiningDate: tempDate,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  ClossConfrimModal = () => {
    this.setState({ confirmpopup: false });
  };
  ConfirmbtnFun = () => {
    let newArray = [...this.state.SwitchStateAll];
    if (this.state.SwitchStateAll[this.state.ToggleIndex] == true) {
      newArray[this.state.ToggleIndex] = false;
      this.SwitchUpdateCall(false);
      this.setState({ SwitchStateAll: newArray });
    } else {
      newArray[this.state.ToggleIndex] = true;
      this.SwitchUpdateCall(true);
      this.setState({ SwitchStateAll: newArray });
    }
    this.setState({
      confirmpopup: false,
      ToggleIndex: null,
    });
  };
  SwitchHandleChange = (index) => {
    if (this.state.SwitchState[index] === false) {
      this.setState({
        confirmpopupmsg:
          "Are you sure to activate " + this.state.TaskOwnerHandle?.Name + " ?",
        confirmpopup: true,
        ToggleIndex: index,
      });
    } else {
      this.setState({
        confirmpopupmsg:
          "Are you sure to deactivate " +
          this.state.TaskOwnerHandle?.Name +
          " ?",
        confirmpopup: true,
        ToggleIndex: index,
      });
    }
    // let newArray = [...this.state.SwitchState];
    // if (this.state.SwitchState[index] == true) {
    //   newArray[index] = false;
    //   this.SwitchUpdateCall(false);
    //   this.setState({ SwitchState: newArray });
    // } else {
    //   newArray[index] = true;
    //   this.SwitchUpdateCall(true);
    //   this.setState({ SwitchState: newArray });
    // }
  };
  //   SwitchHandleChange = (index) => {
  //     let newArray = [...this.state.SwitchState];
  //     if (this.state.SwitchState[index] == true) {

  //       newArray[index] = false;
  //       this.SwitchUpdateCall(false);
  //       this.setState({ SwitchState: newArray });
  //     } else {
  // ;
  //       newArray[index] = true;
  //       this.SwitchUpdateCall(true);
  //       this.setState({ SwitchState: newArray });
  //     }
  //   };
  SetSwitchState = (isActive, temp) => {
    if (isActive == true) {
      temp.push(true);
    } else {
      temp.push(false);
    }
    // console.log(temp);
  };
  CloseModal = () => {
    this.setState({ AddTaskOwnersShow: false });
    this.setState({ TaskOwnerFilterShow: false });
    this.setState({
      TaskOwnerHandle: {
        UserProfileID: 0,
        UserName: "",
        Name: "",
        Department: "",
        Designation: "",
        Status: true,
        Email: "",
        AccountType: "",
        JoiningDate: "",
      },
      error_class: {
        UserName: "",
        Name: "",
        Department: "",
        Designation: "",
        Email: "",
        AccountType: "",
        Status: "",
        JoiningDate: "",
      },
      ModalTitle: null,
      // page: 0,
      // rowsPerPage: 10,
      ShowJoiningDate: false,
    });
  };

  validatePopUp = () => {
    var checkError = false;
    if (this.state.TaskOwnerHandle.UserName === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          UserName: "input_error",
        },
      }));
    }
    if (this.state.TaskOwnerHandle.Name === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          Name: "input_error",
        },
      }));
    }
    // if (
    //   this.state.TaskOwnerHandle.Department === "" ||
    //   this.state.TaskOwnerHandle.Department === 0
    // ) {
    //   checkError = true;
    //   this.setState((prevState) => ({
    //     error_class: {
    //       ...prevState.error_class,
    //       Department: "input_error",
    //     },
    //   }));
    // }
    if (this.state.TaskOwnerHandle.Designation === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          Designation: "input_error",
        },
      }));
    }
    if (this.state.TaskOwnerHandle.Email === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          Email: "input_error",
        },
      }));
    }
    if (this.state.TaskOwnerHandle.AccountType === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          AccountType: "input_error",
        },
      }));
    }
    if (this.state.TaskOwnerHandle.Status === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          Status: "input_error",
        },
      }));
    }

    if (this.state.TaskOwnerHandle.JoiningDate === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          JoiningDate: "input_error",
        },
      }));
    }
    if (
      this.state.TaskOwnerHandle.UserName !== "" ||
      this.state.TaskOwnerHandle.Email !== ""
    ) {
      var checkTask = this.state.lstUser.filter((Name) =>
        Name.UserName.toLowerCase().includes(
          this.state.TaskOwnerHandle?.UserName.toLowerCase()
        )
      );
      var checkTask1 = this.state.lstUser.filter((Name) =>
        Name.Email?.toLowerCase().includes(
          this.state.TaskOwnerHandle?.Email?.trim().toLowerCase()
        )
      );
      if (
        checkTask?.length > 0 &&
        checkTask[0]?.UserProfileTableID !==
          this.state.TaskOwnerHandle?.UserProfileID
      ) {
        checkError = true;
        this.setState({ Title: "User Alert" });
        this.setState({
          errorMsg:
            ` User Name "` +
            this.state.TaskOwnerHandle?.UserName +
            `"  Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask.length > 1) {
          checkError = true;
          this.setState({ Title: "User Alert" });
          this.setState({
            errorMsg:
              ` User Name "` +
              this.state.TaskOwnerHandle?.UserName +
              `"  Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
      if (
        checkTask1.length > 0 &&
        checkTask1[0]?.UserProfileTableID !==
          this.state.TaskOwnerHandle?.UserProfileID
      ) {
        checkError = true;
        this.setState({ Title: "User Alert" });
        this.setState({
          errorMsg:
            ` Email  "` +
            this.state.TaskOwnerHandle?.Email +
            `"   Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask1.length > 1) {
          checkError = true;
          this.setState({ Title: "User Alert" });
          this.setState({
            errorMsg:
              ` Email  "` +
              this.state.TaskOwnerHandle?.Email +
              `"   Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
      if (
        checkTask.length > 0 &&
        checkTask1.length > 0 &&
        checkTask1[0]?.UserProfileTableID !==
          this.state.TaskOwnerHandle?.UserProfileID &&
        checkTask[0]?.UserProfileTableID !==
          this.state.TaskOwnerHandle?.UserProfileID
      ) {
        checkError = true;
        this.setState({ Title: "User Alert" });
        this.setState({
          errorMsg:
            `User Name "` +
            this.state.TaskOwnerHandle?.UserName +
            `"Email  "` +
            this.state.TaskOwnerHandle?.Email +
            `"  Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask.length > 1 && checkTask1.length > 1) {
          checkError = true;
          this.setState({ Title: "User Alert" });
          this.setState({
            errorMsg:
              `User Name "` +
              this.state.TaskOwnerHandle?.UserName +
              `"Email  "` +
              this.state.TaskOwnerHandle?.Email +
              `"  Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
    }
    if (!checkError) {
      this.AddUpdateUserProfile();
    }
  };

  CheckPopUp = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CloseokModal = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  HandelErrorRemove = (name) => {
    if (name == "UserName") {
      if (this.state.TaskOwnerHandle.UserName == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            UserName: "",
          },
        }));
      }
    }
    if (name == "Name") {
      if (this.state.TaskOwnerHandle.Name == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Name: "",
          },
        }));
      }
    }
    if (name == "Department") {
      if (this.state.TaskOwnerHandle.Department == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Department: "",
          },
        }));
      }
    }
    if (name == "Designation") {
      if (this.state.TaskOwnerHandle.Designation == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Designation: "",
          },
        }));
      }
    }
    if (name == "Email") {
      if (this.state.TaskOwnerHandle.Email == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Email: "",
          },
        }));
      }
    }
    if (name == "AccountType") {
      if (this.state.TaskOwnerHandle.AccountType == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            AccountType: "",
          },
        }));
      }
    }
    if (name == "Status") {
      if (this.state.TaskOwnerHandle.Status === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Status: "",
          },
        }));
      }
    }
    if (name == "Date") {
      if (this.state.TaskOwnerHandle.JoiningDate == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            JoiningDate: "",
          },
        }));
      }
    }
  };
  handleChangeSwitch = () => {
    var newSwitchState = this.state.SwitchState.slice(
      this.state.page * this.state.rowsPerPage,
      this.state.page * this.state.rowsPerPage + this.state.rowsPerPage
    );
    this.setState({ SwitchStateAll: newSwitchState });
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage }, () => {
      this.handleChangeSwitch();
    });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  render() {
    return (
      <>
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-4 col-12">
              <h2 class="m_font_heading">Manage Users</h2>
            </div>
            <div className="col-sm-8 col-12 pt-1" id="ems_user_manage_projects">
              {Cookies.get("Role") === "SuperAdmin" ? (
                <button
                  className="btn-black mr-1"
                  type="button"
                  style={{ float: "right" }}
                  onClick={() => {
                    this.setState(
                      {
                        page: 0,
                        TaskOwnerHandleFilter: {
                          Name: "",
                          Department: "",
                          Designation: "",
                          Status: "",
                          Email: "",
                          AccountType: "",
                        },
                      },
                      () => {
                        this.LoadFilterUserProfile();
                      }
                    );
                  }}
                >
                  Clear Search
                  <i className="menu-icon"></i>
                </button>
              ) : (
                ""
              )}
              <button
                className="btn-black mr-1"
                type="button"
                style={{ float: "right" }}
                onClick={() => {
                  this.setState({ TaskOwnerFilterShow: true });
                }}
                data-toggle="modal"
                data-target="#TaskOwnerFilter"
              >
                Filter
                <i className="menu-icon fa fa-plus"></i>
              </button>

              <button
                className="btn-black mr-1"
                type="button"
                style={{ float: "right" }}
                onClick={() => {
                  this.setState({
                    AddTaskOwnersShow: true,
                    ModalTitle: "Add User",
                  });
                }}
                data-toggle="modal"
                data-target="#AddTaskOwners"
              >
                Add User
                <i className="menu-icon fa fa-plus"></i>
              </button>
            </div>
          </div>
          {this.state.Loading ? (
            <Loader />
          ) : (
            <div className="row" style={{ marginTop: "30px" }}>
              <div className="col-12">
                <div className="card p-2">
                  <div className="card-body">
                    <TableContainer
                      className="table-responsive"
                      component={"div"}
                    >
                      <Table className="table" id="ems_manage_users">
                        <TableHead>
                          <TableRow>
                            <TableCell>User Name</TableCell>
                            <TableCell>Full Name</TableCell>
                            <TableCell>Department</TableCell>
                            <TableCell>Designation</TableCell>
                            <TableCell>Email Address</TableCell>
                            <TableCell>Joining Date</TableCell>
                            <TableCell>Account Type</TableCell>
                            <TableCell>Status</TableCell>
                            {/* <TableCell></TableCell> */}
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {this.state.lstUser.length > 0 ? (
                            this.state.lstUser
                              .slice(
                                this.state.page * this.state.rowsPerPage,
                                this.state.page * this.state.rowsPerPage +
                                  this.state.rowsPerPage
                              )
                              .map((item, index) => {
                                return (
                                  <TableRow
                                    style={
                                      Cookies.get("Role") === "SuperAdmin"
                                        ? { cursor: "pointer" }
                                        : { cursor: "" }
                                    }
                                    onClick={() => {
                                      if (Cookies.get("Role") == "SuperAdmin") {
                                        this.setState({
                                          AddTaskOwnersShow: true,
                                          ModalTitle: "Update User",
                                          UserIdForProjects:
                                            item.UserProfileTableID,
                                          TaskOwnerHandle: {
                                            UserProfileID:
                                              item.UserProfileTableID,
                                            UserName: item.UserName,
                                            Name: item.Name,
                                            Department: item.Department_ID,
                                            Designation: item.Designation,
                                            Status: item.Status,
                                            Email: item.Email,
                                            AccountType: item.EMSRole,
                                            JoiningDate: item.JoiningDate,
                                          },
                                          ShowJoiningDate: moment(
                                            new Date(item.JoiningDate)
                                          ).format("DD/MM/YYYY"),
                                        });
                                      }
                                    }}
                                  >
                                    <TableCell>{item.UserName}</TableCell>
                                    <TableCell>{item.Name}</TableCell>
                                    <TableCell>{item.Department}</TableCell>
                                    <TableCell>{item.Designation}</TableCell>
                                    <TableCell>{item.Email}</TableCell>
                                    <TableCell>
                                      {moment(item.JoiningDate).format(
                                        "DD/MM/YYYY"
                                      )}
                                    </TableCell>
                                    <TableCell>{item.EMSRole}</TableCell>
                                    <TableCell>
                                      {Cookies.get("Role") === "SuperAdmin" ? (
                                        <Switch
                                          checked={
                                            this.state.SwitchStateAll[index] ==
                                            true
                                              ? true
                                              : false
                                          }
                                          onChange={() => {
                                            this.setState(
                                              {
                                                TaskOwnerHandle: {
                                                  ...this.state.TaskOwnerHandle,
                                                  UserProfileID:
                                                    item.UserProfileTableID,
                                                  Name: item.Name,
                                                },
                                                AddTaskOwnersShow: false,
                                              },
                                              () => {
                                                this.SwitchHandleChange(index);
                                              }
                                            );
                                          }}
                                          value={
                                            this.state.SwitchStateAll[index]
                                          }
                                          color="primary"
                                          name="SwitchState"
                                          inputProps={{
                                            "aria-label": "primary checkbox",
                                          }}
                                        />
                                      ) : (
                                        <Switch
                                          checked={
                                            this.state.SwitchStateAll[index] ==
                                            true
                                              ? true
                                              : false
                                          }
                                          value={
                                            this.state.SwitchStateAll[index]
                                          }
                                          color="primary"
                                          name="SwitchState"
                                          inputProps={{
                                            "aria-label": "primary checkbox",
                                          }}
                                        />
                                      )}
                                    </TableCell>
                                    {/* <TableCell>
                                    <i
                                      style={{ cursor: "pointer" }}
                                      className="menu-icon fa fa-edit"
                                      onClick={() => {
                                        this.setState({
                                          AddTaskOwnersShow: true,
                                          ModalTitle: "Update User",
                                          TaskOwnerHandle: {
                                            UserProfileID:
                                              item.UserProfileTableID,
                                            UserName: item.UserName,
                                            Name: item.Name,
                                            Department: item.Department,
                                            Designation: item.Designation,
                                            Status: item.Status,
                                            Email: item.Email,
                                            AccountType: item.EMSRole,
                                            JoiningDate: item.JoiningDate,
                                          },
                                          ShowJoiningDate: moment(
                                            new Date(item.JoiningDate)
                                          ).format("DD/MM/YYYY"),
                                        });
                                      }}
                                    ></i>
                                  </TableCell> */}
                                  </TableRow>
                                );
                              })
                          ) : (
                            <TableRow
                              style={{
                                position: "absolute",
                                marginTop: "15px",
                                marginLeft: "405px",
                              }}
                            >
                              No Record Avaliable
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                    </TableContainer>
                    <TablePagination
                      component="div"
                      count={this.state.lstUser.length}
                      page={this.state.page}
                      onChangePage={this.handleChangePage}
                      rowsPerPage={this.state.rowsPerPage}
                      onChangeRowsPerPage={this.handleChangeRowsPerPage}
                    />
                  </div>
                </div>
              </div>
            </div>
          )}
          <AddTaskOwnersPopup
            ModalTitle={this.state.ModalTitle}
            AddTaskOwnersShow={this.state.AddTaskOwnersShow}
            closeAddTaskOwners={() => {
              if (this.state.TaskOwnerHandle.UserProfileID) {
                this.setState({ IsUpdated: true }, () => {
                  this.CloseModal();
                });
              } else {
                this.CloseModal();
              }
            }}
            TaskOwnerHandle={this.state.TaskOwnerHandle}
            DesignationRecord={this.state.lstDesignation}
            DepartmentList={this.state.lstDepartment}
            TaskOwnerHandleChange={this.TaskOwnerHandleChange}
            ShowJoiningDate={this.state.ShowJoiningDate}
            JoinDateHandler={this.JoiningDateHandler}
            // AddUserProfileFun={this.AddUpdateUserProfile}
            UserProjectsShow={this.ShowHideUserProject}
            errorclass={this.state.error_class}
            AddUserProfileFun={this.validatePopUp}
            HandelErrorRemove={this.HandelErrorRemove}
          />
          <TaskOwnerFilterPopup
            TaskOwnerFilterShow={this.state.TaskOwnerFilterShow}
            DesignationRecord={this.state.lstDesignation}
            TaskOwnerHandle={this.state.TaskOwnerHandleFilter}
            DepartmentList={this.state.lstDepartment}
            TaskOwnerHandleChangeFilter={this.TaskOwnerHandleChangeFilter}
            FilterUserFun={this.SearchFiltercheck}
            ClearSearch={this.LoadUserProfile}
            closeTaskOwnerFilter={this.CloseModal}
          />
          <SureMsgPopUp
            Title={this.state.Title}
            ShowMsgs={this.state.errorMsg}
            show={this.state.ShowMsgPopUp}
            onConfrim={this.CheckPopUp}
            onHide={this.CloseokModal}
          />
          <ConfirmPopUp
            Title={"User Status"}
            ShowMsgs={this.state.confirmpopupmsg}
            show={this.state.confirmpopup}
            onConfrim={this.ConfirmbtnFun}
            onHide={this.ClossConfrimModal}
          />
          {this.state.UserProjectsShow ? (
            <ProjectsListModal
              ModalTitle={"Projects List"}
              UserProjectsShow={this.state.UserProjectsShow}
              UserProjectsHide={this.ShowHideUserProject}
              UserIdForProjects={this.state.UserIdForProjects}
            />
          ) : null}
        </div>
      </>
    );
  }
}
export default TaskOwner;
