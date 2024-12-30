import React, { Component } from "react";
import logo from "../../images/rezaid-logo.png";
import logo2 from "../../images/rezaid-logo-2.png";
import { BrowserRouter, Route, Link } from "react-router-dom";
import ReactDOM from "react-dom";
import axios from "axios";
import AddRecord from "../AddRecord/AddRecord";
import moment from "moment";
import Cookies from "js-cookie";
import "./NotificationDDDesign.css";
import ShowNotificationPopUp from "../Notificaion/ShowNotificationPopUp";
import NoPic from "../../images/profile-avator.png";
import FullscreenIcon from "@material-ui/icons/Fullscreen";
import FullscreenExitIcon from "@material-ui/icons/FullscreenExit";
import ZoomOutMapIcon from "@material-ui/icons/ZoomOutMap";
import { NotRefreshAction } from "../Redux/Actions/index";
import { connect } from "react-redux";
import AccessAlarmsIcon from "@material-ui/icons/AccessAlarms";
import { Dropdown, DropdownButton } from "react-bootstrap";
import Tooltip from "@material-ui/core/Tooltip";
import { encrypt, decrypt } from "react-crypt-gsm";
const DDDeleteAnimation = {
  display: "flex",
  transition: "opacity 1s",
  opacity: 0.2,
};
const DDNotificationCard = {
  marginBottom: "1rem",
  cursor: "pointer",
};
class Dashboard extends Component {
  constructor(props) {
    super(props);
    this.state = {
      DDNotificationShow: "dropdown for-notification",
      DDMenu: "dropdown-menu",
      btnNotification: "false",
      DDMenuSetting: "dropdown-menu",
      btnSetting: "false",
      notificationCount: "",
      showModel: false,
      ProjectName: null,
      ProjectDescription: "",
      ProjectId: null,
      /*  MainTask: null, */
      lstTaskOwnerNames: [],
      lstResourceMapping: [],
      /* MapUserList: [], */
      isRefreshGrid: false,
      ariaExpanded: false,
      navbarToggler: "navbar-toggler collapsed",
      showMenuOnMobile: "main-menu navbar-collapse collapse in",
      showSelectedDate: new Date(), //Add Record
      AssignmentDateTime: new Date(), //Add Record
      NotficationList: null,
      unReadNotficationList: [],
      showNotificationModel: false,
      showNotifcationMsg: null,
      showNotifcationHeader: null,
      checkNotificationCard: null,
      StickerHeader: null,
      lstReport: null,
      GrouplstReport: null,
      ProjectIdFilterShow: "",
      ProjectIdFilter: null,
    };
    this.ShowNotification = this.ShowNotification.bind(this);
    this.ShowSetting = this.ShowSetting.bind(this);
    this.HideNotification = this.HideNotification.bind(this);
    this.ShowModel = this.ShowModel.bind(this);
    this.HideModel = this.HideModel.bind(this);
    this.NotificationDDRef = React.createRef();
  }

  componentDidMount() {
    if (Cookies.get("UserID") == null && Cookies.get("UserID") == undefined) {
      window.location.href = ".";
    }
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
    this.setState({ NotificationCardStyle: DDNotificationCard }); //Default Style for notification DD card
    this.LoadResourceMapping();
    this.LoadProjectName();
    this.LoadReportsData();
    /* this.LoadMainTasks(); */
    /*  this.LoadTasksOwnerName(); */
    this.LoadUserProfile();
    this.NotificationCount();
    this.UnReadNotififcation();
    document.addEventListener("mousedown", this.handleClickOutside);
    window.onscroll = () => {
      this.setState({ offset: window.pageYOffset }, () => {
        if (this.state.offset > 20) {
          this.setState({ StickerHeader: "sticky_header" });
        } else {
          this.setState({ StickerHeader: "" });
        }
      });
    };
  }

  componentWillUnmount() {
    document.removeEventListener("mousedown", this.handleClickOutside);
  }

  NotificationCount = () => {
    const NotifyObj = {
      UserId: Cookies.get("UserID"),
    };
    axios({
      method: "post",
      url: `${process.env.REACT_APP_BASE_URL}Notification/NotificationCount`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("access_token"),
        encrypted: localStorage.getItem("EncryptedType"),
        Role_Type: Cookies.get("Role"),
      },
      data: NotifyObj,
    }).then((response) => {
      if (response?.data?.StatusCode === 401) {
        this.logout();
      }
      if (response.data.StatusCode == "200") {
        var res = response.data.Result;
        this.setState({ notificationCount: res });
      }
    });
  };

  LoadResourceMapping = () => {
    try {
      const UserID = {
        UserId: Cookies.get("UserID"),
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/GetResourceMappingList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserID,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstResourceMapping: res.data.Result });
        /*  var tempArray = [];
        if (this.state.lstResourceMapping !== null) {
          this.state.lstResourceMapping.map((item) => {
            var userMappings = item.UserId;
            if (userMappings !== "") {
              var splitArr = userMappings.split(",");
              for (var i = 0; i < splitArr.length; i++) {
                if (Cookies.get("UserID") == splitArr[i]) {
                  tempArray.push(item.ProjectNames);
                  break;
                }
              }
            }
          });
         this.setState({ MapUserList: tempArray }); 
        } */
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadReportsData = () => {
    try {
      this.setState({ ShowFilterTop: true });
      this.setState({ Loading: true, GrouplstReport: [] });
      const objRptParam = {
        FromDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
        IsApproved: null,
        LoggedInUserId:
          Cookies.get("Role") === "Staff" ? Cookies.get("UserID") : -1,
        MainTaskID: -1,
        ProjectId:
          this.state.ProjectIdFilter === null ||
          this.state.ProjectIdFilter === ""
            ? -1
            : this.state.ProjectIdFilter, //"All",
        SubTaskID: -1,
        ToDate: "",
        ClientID: this.state.ClientIDSelected,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Reports/GetAssignments`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },

        data: objRptParam,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
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
        console.log(tempList);
        for (var i = 0; i <= 2; i++) {
          tempList.sort(function (a, b) {
            a = a.AssignmentDateTime.split("/");
            /* a[0] = day
            a[1] = month 
            a[2] = year */
            var _tempa = new Date(a[2], a[1], a[0]);
            b = b.AssignmentDateTime.split("/");
            var _tempb = new Date(b[2], b[1], b[0]);
            return _tempb - _tempa;
          });
        }
        this.setState({ lstReport: tempList });
        this.setState({ ReportCount: _tempCount });
        this.setState({ Loading: false });
        var grouparray = [];
        for (var i = 0; i < res.data.Result.length; i++) {
          if (grouparray.length == 0) {
            var obj = {
              ProjectNameAll: res.data.Result[i]?.ProjectName,
              ProjectIdAll: res.data.Result[i]?.ProjectID,
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
              };
              grouparray.push(obj);
            }
          }
        }

        this.setState({ GrouplstReport: grouparray });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadProjectName = () => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: "",
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ ProjectName: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  /*   LoadMainTasks = () => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
      }).then((res) => {
        this.setState({ MainTask: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  }; */

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
        this.setState({ lstTaskOwnerNames: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  }; */

  LoadUserProfile = () => {
    try {
      var UserIdObj = {
        UserID: 0,
      };
      if (Cookies.get("Role") == "Staff" || Cookies.get("Role") == "Admin") {
        UserIdObj = {
          UserID: Cookies.get("UserID"),
        };
      }

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
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstTaskOwnerNames: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ShowNotification = () => {
    if (this.state.btnNotification == "false") {
      this.setState({
        DDNotificationShow: "dropdown for-notification show",
        DDMenu: "dropdown-menu show",
        btnNotification: "true",
      });
      this.SeenNotification();
    } else {
      this.setState({
        DDNotificationShow: "dropdown for-notification",
        DDMenu: "dropdown-menu",
        btnNotification: "false",
      });
    }
  };
  ShowSetting = () => {
    if (this.state.btnSetting == "false") {
      this.setState({
        DDSettingShow: "dropdown for-notification show",
        DDMenuSetting: "dropdown-menu show",
        btnSetting: "true",
      });
    } else {
      this.setState({
        DDSettingShow: "dropdown for-notification",
        DDMenuSetting: "dropdown-menu",
        btnSetting: "false",
      });
    }
  };

  HideNotification = () => {
    if (
      this.state.btnNotification === "true" ||
      this.state.btnSetting === "true"
    ) {
      this.setState({
        DDNotificationShow: "dropdown for-notification",
        DDMenu: "dropdown-menu",
        btnNotification: "false",
        DDMenuSetting: "dropdown-menu",
        btnSetting: "false",
      });
    }
  };
  ShowModel = (e) => {
    this.LoadProjectName();
    this.LoadReportsData();
    this.setState({
      showSelectedDate: new Date(),
    });
    var tempDate = moment(new Date()).format("MM/DD/YYYY");
    this.setState({ AssignmentDateTime: tempDate });
    this.setState({
      showModel: true,
    });

    e.preventDefault();
  };
  HideModel = () => {
    this.setState({
      showModel: false,
    });
  };
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
      //window.location.href = ".";
    } catch (ee) {
      alert(ee);
    }
  };

  onToggle = () => {
    var node = ReactDOM.findDOMNode(this);
    node.classList.toggle("open");
  };
  toggleNavBar = () => {
    if (this.state.ariaExpanded == true) {
      this.setState({ ariaExpanded: false });
      this.setState({ navbarToggler: "navbar-toggler collapsed" });
      this.setState({
        showMenuOnMobile: "main-menu navbar-collapse collapse in",
      });
    } else {
      this.setState({ ariaExpanded: true });
      this.setState({ navbarToggler: "navbar-toggler" });
      this.setState({
        showMenuOnMobile: "main-menu navbar-collapse collapse show",
      });
    }
  };
  AssignmentDateChangedEvent = (date) => {
    //Add Record
    try {
      this.setState({ showSelectedDate: date });

      var tempDate = moment(date).format("MM/DD/YYYY");
      this.setState({ AssignmentDateTime: tempDate });
    } catch (ex) {
      alert(ex);
    }
  };
  UnReadNotififcation = (ID) => {
    try {
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
        NotificationId: ID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/GetunReadNotification`, // isSeen Notification
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: NotifyObj,
      }).then((response) => {
        if (response?.data?.StatusCode === 401) {
          this.logout();
        }
        if (
          response.data.StatusCode == "200" &&
          response.data.Result.length > 0
        ) {
          this.setState({ NotficationList: response.data.Result });
          var tempArray = [];
          if (this.state.NotficationList !== null) {
            this.state.NotficationList.map((item) => {
              if (item.isRead == 0) {
                tempArray.push({
                  msg: item.NotificationMsg,
                  time: item.CreationDateTime,
                  header: item.NotificationHeader,
                  ID: item.NotificationId,
                });
              }
            });
          }
          this.setState({ unReadNotficationList: tempArray });
          this.setState({ Loading: false });
        }
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  ClearNotification = () => {
    try {
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/GetNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: NotifyObj,
      }).then((response) => {
        if (response?.data?.StatusCode === 401) {
          this.logout();
        }
        this.componentDidMount();
        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  ReadNotification = (ID) => {
    try {
      this.setState({ checkNotificationCard: ID });
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
        NotificationId: ID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/ReadNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: NotifyObj,
      }).then((response) => {
        if (response?.data?.StatusCode === 401) {
          this.logout();
        }
        setTimeout(() => {
          this.componentDidMount();
        }, 500);

        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };

  SeenNotification = (ID) => {
    try {
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
        NotificationId: ID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/SeenNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: NotifyObj,
      }).then((response) => {
        if (response?.data?.StatusCode === 401) {
          this.logout();
        }
        this.componentDidMount();
        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  CloseNotificationModal = () => {
    this.setState({ showNotificationModel: false });
  };
  handleClickOutside = (event) => {
    if (
      this.NotificationDDRef &&
      !this.NotificationDDRef.current.contains(event.target)
    ) {
      this.HideNotification();
    }
  };
  CloseNavBar = () => {
    this.setState({ ariaExpanded: false });
    this.setState({ navbarToggler: "navbar-toggler collapsed" });
    this.setState({
      showMenuOnMobile: "main-menu navbar-collapse collapse in",
    });
  };
  setReduxFalse = () => {
    this.props.NotRefreshAction(); //use to set the redux RefreshBit to false
    this.setState({ key: Math.random() });
  };
  render() {
    return (
      <div>
        <div className="display-table">
          <aside
            id="left-panel"
            className="left-panel"
            style={{ backgroundColor: "#000" }}
          >
            <nav className="navbar navbar-expand-sm navbar-default">
              <div className="navbar-header">
                <button
                  onClick={this.toggleNavBar}
                  className={this.state.navbarToggler}
                  type="button"
                  data-toggle="collapse"
                  data-target="#main-menu"
                  aria-controls="main-menu"
                  aria-expanded={this.state.ariaExpanded}
                  aria-label="Toggle navigation"
                >
                  <i className="fa fa-bars"></i>
                </button>
                {Cookies.get("Role") == "Admin" ||
                Cookies.get("Role") == "SuperAdmin" ? (
                  <>
                    <a className="navbar-brand">
                      <Link to="/AdminDashboard">
                        <img src={logo} alt="Logo" />
                      </Link>
                    </a>
                    <a className="navbar-link">
                      <Link to="/AdminDashboard"> REZAID </Link>
                    </a>
                    <a className="navbar-brand hidden">
                      <Link to="/AdminDashboard">
                        <img src={logo2} alt="Logo" />
                      </Link>
                    </a>
                  </>
                ) : (
                  <>
                    <a className="navbar-brand">
                      <Link to="/UserDashboard">
                        <img src={logo} alt="Logo" />
                      </Link>
                    </a>
                    <a className="navbar-link">
                      <Link to="/UserDashboard"> REZAID </Link>
                    </a>
                    <a className="navbar-brand hidden">
                      <Link to="/UserDashboard">
                        <img src={logo2} alt="Logo" />
                      </Link>
                    </a>
                  </>
                )}
              </div>

              <div
                id="main-menu"
                className={this.state.showMenuOnMobile}
                aria-expanded={this.state.ariaExpanded}
              >
                {Cookies.get("Role") == "Admin" ||
                Cookies.get("Role") == "SuperAdmin" ? (
                  <ul className="nav navbar-nav">
                    <li className="active" onClick={this.toggleNavBar}>
                      <Link to="/AdminDashboard">
                        <Tooltip title="DashBoard">
                          <i className="menu-icon fa fa-home"></i>
                        </Tooltip>
                        Dashboard
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link to="/ReportGrid">
                        <Tooltip title="Reports">
                          <i className="menu-icon fa fa-file-text"></i>
                        </Tooltip>
                        Reports
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link to="/Projects" onClick="">
                        <Tooltip title="Projects">
                          <i className="menu-icon fa fa-folder"></i>
                        </Tooltip>{" "}
                        Projects
                      </Link>
                    </li>

                    {/* <li onClick={this.toggleNavBar} className="show_only_open">
                      <Link to="/MainTask" onClick="">
                        <Tooltip title="Task Management">
                          <i className="menu-icon fa fas fa-tasks"></i>
                        </Tooltip>{" "}
                        Task Management
                      </Link>
                    </li> */}

                    <li onClick={this.toggleNavBar} className="show_only_open">
                      <Link to="/MainTask" onClick="">
                        <Tooltip title="Main Task">
                          <i className="menu-icon fa fas fa-tasks"></i>
                        </Tooltip>{" "}
                        Main Task
                      </Link>
                    </li>

                    <li onClick={this.toggleNavBar} className="show_only_open">
                      <Link to="/SubTask" onClick="">
                        <Tooltip title="Sub Task">
                          <i
                            className="menu-icon fa fas fa-bullseye"
                            aria-hidden="true"
                          ></i>
                        </Tooltip>{" "}
                        Sub Task
                      </Link>
                    </li>

                    <li onClick={this.toggleNavBar} className="show_only_open">
                      <Link to="/MainTaskMapping" onClick="">
                        <Tooltip title="Main Task Mapping">
                          <i className="menu-icon fa fas fa-map"></i>
                        </Tooltip>{" "}
                        Main Task Mapping
                      </Link>
                    </li>

                    <li onClick={this.toggleNavBar} className="show_only_open">
                      <Link to="/ProjectMapping" onClick="">
                        <Tooltip title="Project Mapping">
                          <i className="menu-icon fa fas fa-window-restore"></i>
                        </Tooltip>{" "}
                        Project Mapping
                      </Link>
                    </li>

                    <li onClick={this.toggleNavBar} className="show_only_open">
                      <Link to="/ClientMapping" onClick="">
                        <Tooltip title="Client Mapping">
                          <i className="menu-icon fa fas fa-sun-o"></i>
                        </Tooltip>{" "}
                        Client Mapping
                      </Link>
                    </li>

                    <li className="sr_task_manages">
                      <Dropdown className="menu_dropdown hide_only_open">
                        <Dropdown.Toggle
                          variant="default report_btn_export"
                          id="dropdown-basic"
                          style={{
                            backgroundColor: "transparent",
                          }}
                        >
                          <Link>
                            <Tooltip title="Task Management">
                              <i className="menu-icon fa fas fa-tasks"></i>
                            </Tooltip>
                            Task Management
                          </Link>
                        </Dropdown.Toggle>

                        <Dropdown.Menu className="align_top">
                          <Link
                            onClick={this.toggleNavBar}
                            className="dropdown-item pt-0"
                            to="/MainTask"
                          >
                            Main Task
                          </Link>

                          <Link
                            onClick={this.toggleNavBar}
                            className="dropdown-item"
                            to="/SubTask"
                          >
                            Sub Task
                          </Link>

                          <Link
                            onClick={this.toggleNavBar}
                            className="dropdown-item"
                            to="/MainTaskMapping"
                          >
                            Main Task Mapping
                          </Link>

                          <Link
                            onClick={this.toggleNavBar}
                            className="dropdown-item"
                            to="/ProjectMapping"
                          >
                            Project Mapping
                          </Link>
                          <Link
                            onClick={this.toggleNavBar}
                            className="dropdown-item"
                            to="/ClientMapping"
                          >
                            Client Mapping
                          </Link>
                          <Link
                            onClick={this.toggleNavBar}
                            className="dropdown-item pb-0"
                            to="/DepartmentMapping"
                          >
                            Department Mapping
                          </Link>
                        </Dropdown.Menu>
                      </Dropdown>
                    </li>
                    {/*  <li onClick={this.toggleNavBar}>
                      <Link to="/Projects" onClick="">
                        <i className="menu-icon fa fa-clock-o"></i> Time Log
                      </Link>
                    </li>
                   
                    {/*  <li onClick={this.toggleNavBar}>
                      <Link to="/Projects" onClick="">
                        <i className="menu-icon accessalarms_icon">
                          <AccessAlarmsIcon />
                        </i>
                        Project Schedule
                      </Link>
                    </li> */}
                    <li
                      className="ems_users_tab_mb"
                      onClick={this.toggleNavBar}
                    >
                      <Link to="/TaskOwner" onClick="">
                        <Tooltip title="Users">
                          <i className="menu-icon fa fa-users"></i>
                        </Tooltip>{" "}
                        Users
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link to="/Designation" onClick="">
                        <Tooltip title="Designation">
                          <i className="menu-icon fa fa-id-badge "></i>
                        </Tooltip>{" "}
                        Designation
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link to="/Client" onClick="">
                        <Tooltip title="Client">
                          <i className="menu-icon fa fa-male "></i>
                        </Tooltip>{" "}
                        Client
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link to="/Department" onClick="">
                        <Tooltip title="Department">
                          <i className="menu-icon fa fa-building "></i>
                        </Tooltip>{" "}
                        Department
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link onClick={this.ShowModel}>
                        <Tooltip title="Add Record">
                          <i className="menu-icon fa fa-plus"></i>
                        </Tooltip>{" "}
                        Add Record
                      </Link>
                    </li>
                  </ul>
                ) : (
                  <ul className="nav navbar-nav">
                    <li className="active" onClick={this.toggleNavBar}>
                      <Link to="/UserDashboard">
                        <i className="menu-icon fa fa-home"></i>Dashboard
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link to="/ReportGrid">
                        <i className="menu-icon fa fa-file-text"></i> Reports
                      </Link>
                    </li>
                    <li onClick={this.toggleNavBar}>
                      <Link onClick={this.ShowModel}>
                        <i className="menu-icon fa fa-plus"></i> Add Record
                      </Link>
                    </li>
                  </ul>
                )}
              </div>
            </nav>
            <button
              id="menuToggle"
              className="menutoggle pull-left"
              onClick={this.onToggle}
            >
              <span className="fullscreen">
                <ZoomOutMapIcon />
              </span>

              <span className="fullscreenclose">
                <FullscreenExitIcon />
              </span>
            </button>
            <div className="copyrights">
              <p>
                &copy; {new Date().getFullYear()} Rezaid All rights reserved.
              </p>
            </div>
          </aside>

          <div
            id="right-panel"
            className="right-panel"
            onClick={this.CloseNavBar}
          >
            <header
              id="header"
              className={`header ${this.state.StickerHeader}`}
            >
              <div className="header-menu row flex-sm-nowrap">
                <div className="col-sm-2">
                  {/*  <button
                    id="menuToggle"
                    className="menutoggle pull-left"
                    onClick={this.onToggle}
                  >
                    <span className="fullscreen">
                      <ZoomOutMapIcon />
                    </span>

                    <span className="fullscreenclose">
                      <FullscreenExitIcon />
                    </span>
                  </button> */}
                </div>
                <div className="col-sm-10">
                  <div className="header-actions">
                    <div className="form-inline searh-container">
                      <form className="search-form">
                        <input
                          className="form-control mr-sm-2"
                          type="text"
                          placeholder="Search ..."
                          aria-label="Search"
                        />
                        <button className="search-close" type="submit">
                          <i className="fa fa-close"></i>
                        </button>
                      </form>
                    </div>

                    <div
                      className={this.state.DDNotificationShow}
                      ref={this.NotificationDDRef}
                    >
                      <Dropdown className="notification_dropdwon">
                        <Dropdown.Toggle>
                          <button
                            style={{ verticalAlign: "bottom" }}
                            onClick={this.ShowNotification}
                            className="btn btn-secondary dropdown-toggle position-relative"
                            type="button"
                            id="notification"
                            data-toggle="dropdown"
                            aria-haspopup="true"
                            aria-expanded={this.state.btnNotification}
                          >
                            <i className="fa fa-bell"></i>
                            {this.state.notificationCount == 0 ? (
                              ""
                            ) : (
                              <span className="count bg-danger">
                                {this.state.notificationCount}
                              </span>
                            )}
                          </button>
                        </Dropdown.Toggle>

                        <Dropdown.Menu
                          className={this.state.DDMenu}
                          aria-labelledby="notification"
                          style={{
                            left: "-205px",
                            minWidth: "380px",

                            overflowY: "scroll",
                            borderRadius: "15px",
                            maxHeight: "85vh",
                          }}
                        >
                          <div>
                            <a>
                              <div className="card-header">
                                <h4 style={{ display: "inline-block" }}>
                                  Notifications
                                </h4>
                                <Dropdown.Item>
                                  <span
                                    onClick={() => {
                                      this.ClearNotification();
                                    }}
                                    className="float-right"
                                    style={{
                                      fontSize: "12px",
                                      cursor: "pointer",
                                    }}
                                  >
                                    Clear All
                                  </span>
                                </Dropdown.Item>
                                <Dropdown.Item>
                                  <Link
                                    to={{
                                      pathname: "/notification",
                                      NCFun: {
                                        notificationCounts:
                                          this.NotificationCount,
                                        UnReadNotififcationlist:
                                          this.UnReadNotififcation,
                                      },
                                    }}
                                  >
                                    <span
                                      className="float-right"
                                      onClick={() => {
                                        this.HideNotification();
                                      }}
                                      style={{
                                        fontSize: "12px",
                                        cursor: "pointer",
                                        marginRight: "11px",
                                      }}
                                    >
                                      View All
                                    </span>
                                  </Link>
                                </Dropdown.Item>
                              </div>
                              <div className="clearfix"></div>
                              {this.state.unReadNotficationList.length > 0 ? (
                                this.state.unReadNotficationList.map((item) => {
                                  return (
                                    <div className="col-sm-12">
                                      <div
                                        className="card"
                                        style={
                                          this.state.checkNotificationCard ==
                                          item.ID
                                            ? DDDeleteAnimation
                                            : DDNotificationCard
                                        }
                                      >
                                        <div
                                          className="card-header"
                                          style={{
                                            borderBottom: ".7px solid",
                                            padding: "0.25rem",
                                            paddingLeft: "0.5rem",
                                            paddingRight: "0.5rem",
                                            fontWeight: "bold",
                                          }}
                                        >
                                          <span>{item.header}</span>
                                          <span
                                            className="float-right"
                                            style={{ cursor: "pointer" }}
                                          >
                                            <i
                                              onClick={() => {
                                                this.ReadNotification(item.ID);
                                              }}
                                              class="fa fa-times"
                                              aria-hidden="true"
                                            ></i>
                                          </span>
                                        </div>
                                        <div
                                          className="card-body"
                                          style={{ padding: "0rem" }}
                                          onClick={() => {
                                            this.setState({
                                              showNotifcationMsg: item.msg,
                                            });
                                            this.setState({
                                              showNotifcationHeader:
                                                item.header,
                                            });
                                            this.setState({
                                              showNotificationModel: true,
                                            });

                                            this.ReadNotification(item.ID);
                                          }}
                                        >
                                          <div class="module overflow">
                                            <p>{item.msg}</p>
                                          </div>
                                        </div>
                                        <div
                                          className="card-footer"
                                          style={{
                                            borderTop: "none",
                                            padding: "0rem",
                                            paddingLeft: "10px",
                                          }}
                                        >
                                          <span style={{ fontSize: "13px" }}>
                                            {item.time}
                                          </span>
                                        </div>
                                      </div>
                                    </div>
                                  );
                                })
                              ) : (
                                <div className="col-sm-12">
                                  <div className="card">
                                    <div className="card-body">
                                      You have received{" "}
                                      {this.state.notificationCount}{" "}
                                      notification click to view
                                    </div>
                                  </div>
                                </div>
                              )}
                            </a>
                          </div>
                        </Dropdown.Menu>
                      </Dropdown>

                      <div
                        className="dropdown for-message"
                        style={{ display: "none" }}
                      >
                        <button
                          onClick={this.ShowSetting}
                          className="btn btn-secondary dropdown-toggle"
                          type="button"
                          id="message"
                          data-toggle="dropdown"
                          aria-haspopup="true"
                          aria-expanded={this.state.btnSetting}
                        ></button>
                      </div>

                      <div className="dropdown for-message dropdown-profile-setting">
                        <Dropdown menuAlign="right">
                          <Dropdown.Toggle>
                            <button
                              onClick={this.ShowSetting}
                              className="btn btn-secondary dropdown-toggle"
                              type="button"
                              id="message"
                              data-toggle="dropdown"
                              aria-haspopup="true"
                              aria-expanded={this.state.btnSetting}
                            >
                              <i className="fa fa-cog"></i>
                            </button>
                          </Dropdown.Toggle>

                          <Dropdown.Menu
                            className={this.state.DDMenuSetting}
                            aria-labelledby="message"
                            style={{
                              borderRadius: "10px",
                              minWidth: "0px",
                              left: "-30px",
                            }}
                          >
                            <div>
                              {Cookies.get("Role") == "SuperAdmin" ? (
                                <>
                                  <a className="dropdown-item media">
                                    <Dropdown.Item>
                                      <Link to="/EmsSetting">
                                        <p
                                          onClick={() => {
                                            this.HideNotification();
                                          }}
                                        >
                                          EMS Setting
                                        </p>
                                      </Link>
                                    </Dropdown.Item>
                                  </a>
                                  {/*   <a className="dropdown-item media">
                                    <Dropdown.Item>
                                      <Link to="/Maintask">
                                        <p
                                          onClick={() => {
                                            this.HideNotification();
                                          }}
                                        >
                                          Maintask
                                        </p>
                                      </Link>
                                    </Dropdown.Item>
                                  </a>
                                  <a className="dropdown-item media">
                                    <Dropdown.Item>
                                      <Link to="/SubTask">
                                        <p
                                          onClick={() => {
                                            this.HideNotification();
                                          }}
                                        >
                                          SubTask
                                        </p>
                                      </Link>
                                    </Dropdown.Item>
                                  </a> */}
                                </>
                              ) : null}
                              <a className="dropdown-item media">
                                <Dropdown.Item>
                                  <Link to="/UserProfile">
                                    <p
                                      onClick={() => {
                                        this.HideNotification();
                                      }}
                                    >
                                      User Profile
                                    </p>
                                  </Link>
                                </Dropdown.Item>
                              </a>
                              <a className="dropdown-item media">
                                <Dropdown.Item>
                                  <Link to="/ChangePassword">
                                    <p
                                      onClick={() => {
                                        this.HideNotification();
                                      }}
                                    >
                                      Change Password
                                    </p>
                                  </Link>
                                </Dropdown.Item>
                              </a>
                              <a
                                className="dropdown-item media"
                                onClick={this.logout}
                              >
                                <Dropdown.Item>
                                  <Link>
                                    <p
                                      onClick={() => {
                                        this.HideNotification();
                                      }}
                                    >
                                      Logout
                                    </p>
                                  </Link>
                                </Dropdown.Item>
                              </a>
                            </div>
                          </Dropdown.Menu>
                        </Dropdown>
                      </div>
                      <div className="user-area float-right">
                        <Link to="/UserProfile">
                          <img
                            className="user-avatar rounded-circle"
                            src={`https://rezaidems-001-site5.dtempurl.com/userimages/UserImgID${Cookies.get(
                              "UserID"
                            )}.png?cache=${new Date()}`}
                            onError={(e) => {
                              e.target.onerror = null;
                              e.target.src = NoPic;
                            }}
                            //style={{ backgroundImage: `url(${NoPic})` }}
                          />
                          <div className="user_header_box">
                            <span className="user-name">
                              {Cookies.get("UserName")}
                            </span>
                            {Cookies.get("Designation") !== "null" ? (
                              <span>{Cookies.get("Designation")}</span>
                            ) : (
                              ""
                            )}
                          </div>
                        </Link>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </header>
            {this.props.children}
          </div>

          <AddRecord
            show={this.state.showModel}
            hide={this.HideModel}
            Name={this.state.ProjectName}
            ProjectDDChange={this.ReportProjectDDPopUpChange}
            DescriptionText={this.state.ProjectDescription} // need to remove from there
            ProjectID={this.state.ProjectId} // need to remove from there
            /*  Maintasks={this.state.MainTask} */
            Reportlist={this.state.lstReport}
            TaskOwnerNameRecord={this.state.lstTaskOwnerNames}
            ProjectNameMapped={this.state.lstResourceMapping}
            showSelectedDates={this.state.showSelectedDate}
            AssignmentDateTime={this.state.AssignmentDateTime}
            AssignmentDateTimes={this.AssignmentDateChangedEvent}
          />
          <ShowNotificationPopUp
            Title={this.state.showNotifcationHeader}
            errorMsgs={this.state.showNotifcationMsg}
            show={this.state.showNotificationModel}
            onHide={this.CloseNotificationModal}
          />
        </div>
        {this.props.Refresh == true ? this.setReduxFalse() : null}
      </div>
    );
  }
}
const mapStateToProps = (state) => {
  return {
    Refresh: state.RefreshBit,
  };
};
const mapDispatchToProps = (dispatch) => {
  return {
    NotRefreshAction: () => dispatch(NotRefreshAction()),
  };
};
export default connect(mapStateToProps, mapDispatchToProps)(Dashboard);
