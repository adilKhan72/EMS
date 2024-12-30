import React, { Component } from "react";
import { Bar, Doughnut, Pie } from "react-chartjs-2";
import BlogContributionPopup from "./BlogContributionPopup";
import moment from "moment";
import axios from "axios";
import NoPic from "../../../images/profile-avator.png";
import UpdateProjectPopUp from "./UpdateProjectPopUp";
import PopUpModal from "../PopUpModalAdmin/PopUpModal";
import { Dropdown } from "react-bootstrap";
import Cookies from "js-cookie";
import Loader from "../Loader/Loader";
import { encrypt, decrypt } from "react-crypt-gsm";
const errorstyle = {
  borderStyle: "solid",
  borderWidth: "2px",
  borderColor: "Red",
};
var chart = {};
class ProjectDetails extends Component {
  constructor(props) {
    super(props);
    this.state = {
      loading: true,
      blogContributionShow: false,
      DoughnutState: {
        DoughnutLabels: [],
        DoughnutDataDays: [],
        DoughnutDataHours: [],
        DoughnutDays: null,
        DoughnutName: "",
      },

      //incoming Project data from ink
      ProjectData: {
        ProjectID: null,
        ProjectName: "",
        StartDate: null,
        EndData: null,
        Status: 0,
        ProjectType: "",
        ProjectOwner: "",
        ProjectDescription: "",
        Progress: null,
        TotalHours: 0,
        ClientName: null,
        ClientID: 0,
      },
      //// close

      TimeSpent: {
        TS_DayArray: [],
        TS_HoursArray: [],
        TS_MainTaskNames: [],
      },

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
      monthBudgetSelect: moment(new Date()).format("MMMM"),
      DayHourToggleBar: {
        dayStyle: "btn_underline",
        hourStyle: "",
      },
      DayHourToggleDoughnut: {
        dayStyle: "btn_underline",
        hourStyle: "",
      },
      GraphdataFormat: "Days",
      GraphdataFormatDoughnut: "Days",
      //update Project
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
      AddProjectField: {
        ProjectID: -1,
        ProjectName: "",
        StartDate: "",
        EndDate: "",
        Status: "",
        TaskOwner: "",
        ProjectType: "",
        ProjectDescription: "",
      },
      errorStyle: {
        ProjectName: null,
        StartDate: null,
        EndDate: null,
        Status: null,
        TaskOwner: null,

        ProjectType: null,
        ProjectDescription: null,
      },
      currentBudgetYear: new Date(),
      PopUpBit: false,
      Title: "",
      errorMsg: "",
      MonthDDDate: null,
      BarColorIndex: -1,
      BarLabelOnClick: "",
      lstSubTaskTimeOwnerSpent: null,
      ShowTaskOwnerTitle: "",
      ShowTaskOwnerName: "",
      loading: true,
      DDPStartDate: null,
      DDPEndDate: null,
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

    if (this.props.location.ProjectData !== undefined) {
      this.setState({ ProjectData: this.props.location.ProjectData }, () => {
        localStorage.setItem(
          "projectDetailID",
          this.state.ProjectData.ProjectID
        );
        localStorage.setItem(
          "StartDate",
          this.props.location?.ProjectData?.StartDate
        );
        localStorage.setItem(
          "EndData",
          this.props.location?.ProjectData?.EndData
        );
        this.LoadAdminDashobardGrid();
        this.LoadAdminTimeSpent();
        this.lstProjectBudget();
        this.LoadSubTaskTimeSpent();
        this.LoadSubTaskTimeOwnerSpent();
      });
    } else if (this.state.ProjectDetailData !== null) {
      this.LoadAdminDashobardGrid();
    } else {
      this.setState(
        {
          ProjectData: {
            ...this.state.ProjectData,
            ProjectID: localStorage.getItem("projectDetailID"),
          },
        },
        () => {
          this.LoadAdminTimeSpent();
          this.lstProjectBudget();
          this.LoadSubTaskTimeSpent();
          this.LoadSubTaskTimeOwnerSpent();
        }
      );
    }

    console.log(this.props.location.ProjectData);
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
  LoadAdminDashobardGrid = () => {
    try {
      var StartDate = "";
      var EndDate = "";
      this.setState({ Loading: true });
      if (
        (StartDate == "" || StartDate == "Invalid date") &&
        (EndDate == "" || EndDate == "Invalid date")
      ) {
        var tempDate = new Date();
        StartDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
        EndDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
      }
      var ProjectIdAsInt = parseInt(
        localStorage.getItem("projectDetailID"),
        10
      );
      const AdminDataObj = {
        RequestStartDate: StartDate,
        RequestEndDate: EndDate,
        ProjectID: ProjectIdAsInt,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetDashBoardStats_byProjectID`,
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
        const Obj = {
          ProjectID: res.data.Result[0]?.ProjectID,
          ProjectName: res.data.Result[0]?.ProjectName,
          StartDate: res.data.Result[0]?.StartDate,
          EndData: res.data.Result[0]?.EndDate,
          Status: res.data.Result[0]?.isActive,
          ProjectType: res.data.Result[0]?.ProjectType,
          ProjectOwner: res.data.Result[0]?.ProjectOwner,
          ProjectDescription: res.data.Result[0]?.ProjectDescription,
          Progress: res.data.Result[0]?.CompletedPercentage,
          ClientID: res.data.Result[0]?.ClientID,
          ClientName: res.data.Result[0]?.ClientName,
          TotalHours: res.data.Result[0]?.Total_Hours_Spent,
        };
        this.setState({ ProjectData: Obj }, () => {
          this.LoadAdminTimeSpent();
          this.lstProjectBudget();
          this.LoadSubTaskTimeSpent();
          this.LoadSubTaskTimeOwnerSpent();
        });
        this.setState({ Loading: false });
      });
    } catch (e) {}
  };
  CloseUpdateProjectModal = () => {
    this.setState({ UpdateProjectPopBit: false });
    this.setState({ currentBudgetYear: new Date() });
  };
  CloseImageModal = () => {
    this.setState({ ImageModalPopBit: false });
  };
  OpenImageModal = () => {
    this.setState({ ImageModalPopBit: true });
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
    this.setState({
      AddProjectField: {
        ...this.state.AddProjectField,
        StartDate: date,
      },
    });
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
        var tempStartDate = this.state.AddProjectField.StartDate;
        while (
          dt <=
          moment(new Date(this.state.AddProjectField.EndDate)).format("MM")
        ) {
          arr.push(moment(new Date(tempStartDate)).format("MMMM"));
          tempStartDate = moment(tempStartDate).add(1, "M");
          ++dt;
        }
        this.setState({ monthArray: arr });
      }
    );
  };
  /* CheckValidationUpdateProject = (e) => {
    if (this.state.AddProjectField.ProjectName == "") {
      this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Project Name" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectName: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.AddProjectField.StartDate == "") {
      this.setState({ Title: "Invalid Entry" });
      this.setState({
        errorMsg: "Please enter Start Date",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          StartDate: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.AddProjectField.EndDate == "") {
      this.setState({ Title: "Invalid Entry" });
      this.setState({
        errorMsg: "Please enter Start Date",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          EndDate: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.AddProjectField.TaskOwner == "") {
      this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Task Owner Name" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          TaskOwner: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.AddProjectField.ProjectType == "") {
      this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please select Project Type" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectType: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.AddProjectField.ProjectDescription == "") {
      this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Project Description" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ProjectDescription: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else {
      this.updateProject();
    }
  }; */

  errorStyleRemovedAddProject = () => {
    this.setState({ errorStyle: {} });
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
  OpenUpdateProjectModal = () => {
    this.setState({ UpdateProjectPopBit: true }, () => {
      this.lstProjectBudget();
    });
  };
  /* clickHandler = (evt) => {
    const points = Doughnut.getElementAtEventForMode(
      evt,
      "nearest",
      { intersect: true },
      true
    );
    alert(points);
    console.log(points);

    if (points.length) {
      const firstPoint = points[0];
      var label = Doughnut.data.labels[firstPoint._index];
      var value =
        Doughnut.data.datasets[firstPoint._datasetIndex].data[
          firstPoint._index
        ];
    }
  }; */
  /*  handleClick = (e) => {
    var element = this.getElementAtEvent(e);
    // changes only the color of the active object
    this.active[0]._chart.config.data.datasets[0].backgroundColor = "red";
    this.setState({
      DoughnutState: {
        DoughnutLabels: ["RM", "Designs", "QA", "Dev"],
        DoughnutData: [6, 4, 2, 5],
        DoughnutDays: 35,
        DoughnutName: "Total Tasks",
      },
    });
  }; */
  handleMonthBudgetProjectShow = (e) => {
    try {
      this.setState(
        {
          monthBudgetSelect: e.target.value,
        },
        () => {
          this.setState({ BarLabelOnClick: "" });
          this.setState({ BarColorIndex: -1 });
          var text;
          var tempDate = new Date();
          switch (this.state.monthBudgetSelect) {
            case "All":
              this.setState(
                {
                  DDPStartDate: `${tempDate.getFullYear()}-${1}-${1}`,
                  DDPEndDate: tempDate,

                  MonthDDDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "January":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${1}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "February":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${2}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "March":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${3}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "April":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${4}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "May":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${5}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "June":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${6}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "July":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${7}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "August":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${8}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "September":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${9}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "October":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${10}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "November":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${11}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            case "December":
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${12}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
              break;
            default:
              this.setState(
                {
                  MonthDDDate: `${tempDate.getFullYear()}-${1}-${1}`,
                  DDPStartDate: null,
                  DDPEndDate: null,
                },
                () => {
                  this.LoadProjectGrid();
                  this.LoadAdminTimeSpent();
                  this.LoadSubTaskTimeSpent();
                  this.LoadSubTaskTimeOwnerSpent();
                }
              );
          }
        }
      );
    } catch (ex) {
      alert(ex);
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
  LoadAdminTimeSpent = () => {
    try {
      var StartDate = "";
      var EndDate = "";
      if (this.state.DDPEndDate != null && this.state.DDPStartDate != null) {
        StartDate = moment(this.state.DDPStartDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.DDPEndDate).format("YYYY/MM/DD");
      }
      if (this.state.MonthDDDate != null && this.state.MonthDDDate != null) {
        StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      }
      // var StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      // var EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      if (
        (StartDate == "" || StartDate == "Invalid date") &&
        (EndDate == "" || EndDate == "Invalid date")
      ) {
        var tempDate = new Date();
        StartDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
        EndDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
      }
      var ProjectIdAsInt = parseInt(this.state.ProjectData.ProjectID, 10);
      const UserDataObj = {
        ProjectId: ProjectIdAsInt,
        StartDate: StartDate,
        EndDate: EndDate,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetSpentTaskTime`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserDataObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState(
          {
            TimeSpent: {
              TS_DayArray: res.data.Result.DayArr,
              TS_HoursArray: res.data.Result.HoursArr,
              TS_MainTaskNames: res.data.Result.MainTaskNames,
            },
          },
          () => {
            this.setState({
              TimeSpentDataCount: this.state.TimeSpent.TS_MainTaskNames.length,
            });
          }
        );
      });
    } catch (e) {
      //alert(e);
    }
  };
  lstProjectBudget = () => {
    try {
      this.setState({ Loading: true });
      var currentBudgetYear = moment(this.state.currentBudgetYear).format(
        "MM/DD/YYYY"
      );
      const projectBudgetDetailObj = {
        ID: this.state.ProjectData.ProjectID,
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
        this.setState({ Loading: false });
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
                this.setState({ reload: true }, () =>
                  this.setState({ reload: false })
                );
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
  onConfrim = () => {
    this.updateProject();
  };
  CloseModal = () => {
    if (this.state.Title == "SUCCESS") {
      this.setState({
        AddProjectField: {
          ProjectName: "",
          StartDate: "",
          EndDate: "",
          TaskOwner: "",
          ProjectType: "",
          status: null,
          isActive: "",
          ProjectDescription: "",
        },
      });
      this.setState({ currentBudgetYear: new Date() });
      /*  this.setState({
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
      }); */

      this.LoadProjectGrid(); //for update refresh
      this.lstProjectBudget(); // refresh monthBudget
      localStorage.setItem("ProjectImg", null);

      this.setState({ UpdateProjectPopBit: false });
    }
    this.setState({ PopUpBit: false });
  };
  LoadProjectGrid = () => {
    try {
      var StartDate = "";
      var EndDate = "";
      if (this.state.DDPEndDate != null && this.state.DDPStartDate != null) {
        StartDate = moment(this.state.DDPStartDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.DDPEndDate).format("YYYY/MM/DD");
      } else {
        StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      }

      var ProjectIdAsInt = this.state.ProjectData.ProjectID;

      const AdminDataObj = {
        RequestStartDate: StartDate,
        RequestEndDate: EndDate,
        ProjectID: ProjectIdAsInt,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetDashBoardStats_byProjectID`,
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
        // this.setState({
        //   DDPStartDate: null,
        //   DDPEndDate: null,
        // });
        if (res.data.Result.length > 0) {
          this.setState({
            ProjectData: {
              ...this.state.ProjectData,
              ProjectName: res.data.Result[0]?.ProjectName,
              StartDate: res.data.Result[0]?.StartDate,
              EndData: res.data.Result[0]?.EndDate,
              Status: res.data.Result[0]?.isActive,
              ProjectType: res.data.Result[0]?.ProjectType,
              ProjectOwner: res.data.Result[0]?.ProjectOwner,
              ProjectDescription: res.data.Result[0]?.ProjectDescription,
              Progress: res.data.Result[0]?.CompletedPercentage,
              ClientID: res.data.Result[0]?.ClientID,
              ClientName: res.data.Result[0]?.ClientName,
              TotalHours: res.data.Result[0]?.Total_Hours_Spent,
            },
          });
        } else {
          this.setState({
            ProjectData: { ...this.state.ProjectData, Progress: 0 },
          });
        }
      });
    } catch (e) {
      //alert(e);
    }
  };
  LoadSubTaskTimeSpent = () => {
    try {
      var StartDate = "";
      var EndDate = "";
      if (this.state.DDPEndDate != null && this.state.DDPStartDate != null) {
        StartDate = moment(this.state.DDPStartDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.DDPEndDate).format("YYYY/MM/DD");
      }
      if (this.state.MonthDDDate != null && this.state.MonthDDDate != null) {
        StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      }
      // var StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      // var EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      if (
        (StartDate == "" || StartDate == "Invalid date") &&
        (EndDate == "" || EndDate == "Invalid date")
      ) {
        var tempDate = new Date();
        StartDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
        EndDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
      }
      var ProjectIdAsInt = parseInt(this.state.ProjectData.ProjectID, 10);

      const DataObj = {
        ProjectId: ProjectIdAsInt,
        MaintaskName: this.state.BarLabelOnClick,
        StartDate: StartDate,
        EndDate: EndDate,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetSubTaskTime`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DataObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          DoughnutState: {
            DoughnutDataDays: res.data.Result.DayArr,
            DoughnutDataHours: res.data.Result.HoursArr,
            DoughnutLabels: res.data.Result.SubTaskNames,
            DoughnutDays:
              res.data.Result.DayArr.length > 1
                ? parseFloat(
                    res.data.Result.DayArr?.reduce(
                      (a, v) => (a = a + v),
                      0
                    ).toFixed(2)
                  )
                : res.data.Result.DayArr[0],
            DoughnutName:
              res.data.Result.SubTaskNames.length > 1
                ? "All"
                : res.data.Result.SubTaskNames[0],
          },
        });
      });
    } catch (e) {
      //alert(e);
    }
  };

  LoadSubTaskTimeOwnerSpent = () => {
    try {
      var StartDate = "";
      var EndDate = "";
      if (this.state.DDPEndDate != null && this.state.DDPStartDate != null) {
        StartDate = moment(this.state.DDPStartDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.DDPEndDate).format("YYYY/MM/DD");
      }
      if (this.state.MonthDDDate != null && this.state.MonthDDDate != null) {
        StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
        EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      }
      // var StartDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      // var EndDate = moment(this.state.MonthDDDate).format("YYYY/MM/DD");
      if (
        (StartDate == "" || StartDate == "Invalid date") &&
        (EndDate == "" || EndDate == "Invalid date")
      ) {
        var tempDate = new Date();
        StartDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
        EndDate = moment(
          new Date(`${tempDate.getFullYear()}-${tempDate.getMonth() + 1}-${1}`)
        ).format("YYYY/MM/DD");
      }
      var ProjectIdAsInt = parseInt(this.state.ProjectData.ProjectID, 10);

      const DataObj = {
        ProjectId: ProjectIdAsInt,
        MaintaskName: this.state.BarLabelOnClick,
        StartDate: StartDate,
        EndDate: EndDate,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetSubTaskTimeOwner`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DataObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstSubTaskTimeOwnerSpent: res.data.Result,
        });
      });
    } catch (e) {
      //alert(e);
    }
  };
  /*   getUniqueListBy = (arr, key) => {
    return [...new Map(arr.map((item) => [item[key], item])).values()];
  }; */
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
    if (this.state.AddProjectField.TaskOwner == "") {
      /*  this.setState({ Title: "Invalid Entry" });
      this.setState({ errorMsg: "Please enter Task Owner Name" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          TaskOwner: errorstyle,
        },
      }); */
      checkError = true;
      this.setState((prevState) => ({
        errorStyle: {
          ...prevState.errorStyle,
          TaskOwner: "input_error",
        },
      }));
      //this.setState({ PopUpBit: true });
      e.preventDefault();
    }
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
    if (!checkError) {
      this.addProject();
    }
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
    if (name == "ProjectName") {
      if (this.state.SettingName == "") {
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
      if (this.state.AddProjectField.TaskOwner == "") {
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
    // if (this.state.AddProjectField.TaskOwner == "") {
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
    } else {
      this.updateProject();
      //   this.setState({ Title: "SUCCESS" });
      //   this.setState({
      //     errorMsg: "Project Update Successfully",
      //   });
      //   this.setState({ PopUpBit: true });
      //   this.setState({ checkPopUp: "save" });
      //   this.setState({ ConfrimMsgPopUp: true, RecordUpdateStatus: true });
    }
    // e.preventDefault();
  };
  render() {
    const Doughnutstate = {
      labels: this.state.DoughnutState.DoughnutLabels,

      datasets:
        this.state.GraphdataFormatDoughnut == "Days"
          ? [
              {
                label: "Days",
                data: this.state.DoughnutState.DoughnutDataDays,
                backgroundColor: "#0080FF",
                borderColor: "#fff",
                borderWidth: 1,
                hoverBorderWidth: 4,
                hoverBackgroundColor: "#6badff",
                hoverBorderColor: "#6badff",
                borderAlign: "inner",
                offset: 6,
                hoverOffset: 6,
              },
            ]
          : [
              {
                label: "Days",
                data: this.state.DoughnutState.DoughnutDataDays,
                backgroundColor: "#0080FF",
                borderColor: "#fff",
                borderWidth: 1,
                hoverBorderWidth: 4,
                hoverBackgroundColor: "#6badff",
                hoverBorderColor: "#6badff",
                borderAlign: "inner",
                offset: 6,
                hoverOffset: 6,
              },
            ],
    };

    //bar chart
    var backgroundColorDay = [];
    var backgroundColorHour = [];
    var i = 0;

    if (this.state.BarColorIndex > -1 && this.state.GraphdataFormat == "Days") {
      while (this.state.TimeSpent.TS_DayArray.length > i) {
        backgroundColorDay.push("#474747");
        ++i;
      }
      backgroundColorDay[this.state.BarColorIndex] = "#0080FF";
    } else if (
      this.state.BarColorIndex > -1 &&
      this.state.GraphdataFormat == "Hours"
    ) {
      while (this.state.TimeSpent.TS_DayArray.length > i) {
        backgroundColorHour.push("#8d9196");
        ++i;
      }
      backgroundColorHour[this.state.BarColorIndex] = "#0080FF";
    } else {
      while (this.state.TimeSpent.TS_DayArray.length > i) {
        backgroundColorDay.push("#474747");
        backgroundColorHour.push("#8d9196");
        ++i;
      }
    }

    console.log(backgroundColorDay);
    const Dropdownvalue = this.state.monthBudgetSelect;
    const Barstate = {
      labels: this.state.TimeSpent.TS_MainTaskNames,

      datasets:
        this.state.GraphdataFormat == "Days"
          ? [
              {
                label: "Days",
                data: this.state.TimeSpent.TS_DayArray,
                backgroundColor: backgroundColorDay,
              },
            ]
          : this.state.GraphdataFormat == "Hours"
          ? [
              {
                label: "Hours",
                data: this.state.TimeSpent.TS_HoursArray,
                borderColor: "rgba(0,0,0,0.09)",
                borderWidth: 0,
                backgroundColor: backgroundColorHour,
              },
            ]
          : [
              {
                label: "Days",
                data: this.state.TimeSpent.TS_DayArray,
                backgroundColor: backgroundColorDay,
              },
            ],
    };

    var progressbarStyle = "";
    this.state.ProjectData.Progress <= 100
      ? (progressbarStyle = "progress-bar bg-gradient")
      : (progressbarStyle = "progress-bar bg-danger");

    var TaskOwnerNameList = null;
    TaskOwnerNameList =
      this.state.DoughnutState.DoughnutDataDays !== null
        ? this.state.DoughnutState.DoughnutDataDays.map((item, index) => {
            return (
              <tr>
                <td width="10%">
                  {this.state.DoughnutState.DoughnutDataDays[index]}
                </td>
                <td>{this.state.DoughnutState.DoughnutLabels[index]}</td>
                <td className="ems_contributors">
                  <div class="d-flex flex-wrap">
                    {this.state.lstSubTaskTimeOwnerSpent !== null
                      ? this.state.lstSubTaskTimeOwnerSpent.map((obj) => {
                          if (
                            this.state.DoughnutState.DoughnutLabels[index] ==
                            obj.TaskName
                          ) {
                            return (
                              <button type="button" class="btn-black mr-1 mb-1">
                                {obj.TaskOwnerName}
                              </button>
                            );
                          }
                        })
                      : ""}
                  </div>
                </td>
                <td>
                  <i
                    class="ti-more-alt ico-icon cursor-pointer"
                    onClick={() => {
                      var temparray = [];
                      var tempName = "";
                      if (this.state.lstSubTaskTimeOwnerSpent !== null) {
                        this.state.lstSubTaskTimeOwnerSpent.map((obj) => {
                          if (
                            this.state.DoughnutState.DoughnutLabels[index] ==
                            obj.TaskName
                          ) {
                            temparray.push(obj.TaskOwnerName);
                            tempName = obj.TaskName;
                          }
                        });
                      }
                      this.setState({ ShowTaskOwnerTitle: tempName });
                      this.setState({ ShowTaskOwnerName: temparray });
                      this.setState({ blogContributionShow: true });
                    }}
                    type="button"
                    data-toggle="modal"
                    data-target="#BlogContribution"
                    style={{ float: "left" }}
                  ></i>
                </td>
              </tr>
            );
          })
        : "";

    /*    this.state.lstSubTaskTimeOwnerSpent !== null
        ? this.state.lstSubTaskTimeOwnerSpent.map((item,index) => {
            return (
              <tr>
                {this.state.TimeSpent.TS_MainTaskNames[index] == item.TaskName ? }
                        <td width="10%">{this.state.TimeSpent.TS_DayArray[index]}</td>
                        <td>{this.state.TimeSpent.TS_DayArray[index]}</td>
                        <td>
                          <div class="d-flex flex-wrap">
                          <button type="button" class="btn-black mr-1 mb-1">
                            {item.TaskOwnerName}
                           </button>
                          </div>
                        </td>
                        <td>
                          <i
                            class="ti-more-alt ico-icon cursor-pointer"
                            onClick={() => {
                              this.setState({ blogContributionShow: true });
                            }}
                            type="button"
                            data-toggle="modal"
                            data-target="#BlogContribution"
                            style={{ float: "left" }}
                          ></i>
                        </td>
                      </tr>
              
            );
          })
        : ""; */
    return (
      <>
        {this.state.Loading ? (
          <Loader />
        ) : (
          <div class="content mt-3">
            <h3 class="font-weight-semibold m_font_heading mb-2">
              Project Details
            </h3>
            <div class="row">
              <div class="col-sm-12">
                <div class="card">
                  {/* <div class="card-header" style={{ paddingTop: "20px" }}>
                  <div class="row align-items-center">
                    <div class="col-6">
                      <h3 class="font-weight-semibold m_font_heading">
                        Project Details
                      </h3>
                    </div>

                    <div class="col-6 text-right">
                      <button type="button" class="btn-black">
                        Edit Project <i class="fa fa-pencil"></i>
                      </button>
                    </div>
                  </div>
                </div> */}
                  <div class="card-body">
                    <div className="row">
                      <div className="col-sm-1 text-center">
                        <figure
                          class="project-avator project-avator-detail"
                          style={{ float: "left" }}
                        >
                          <img
                            src={`https://rezaidems-001-site5.dtempurl.com//ProjectImages/ProjectImgID${
                              this.state.ProjectData.ProjectID
                            }.png?cache=${new Date()}`}
                            onError={(e) => {
                              e.target.onerror = null;
                              e.target.src = NoPic;
                            }}
                            alt=""
                          />
                        </figure>
                        <div
                          className="d-block d-sm-none"
                          style={{
                            fontSize: "14px",
                            float: "left",
                            padding: "9px 0 0 10px",
                            fontWeight: "600",
                          }}
                        >
                          {this.state.ProjectData.ProjectName}
                        </div>
                      </div>
                      <div className="col-sm-11">
                        <h4 class="mt-3 mb-3 mobile_margin">
                          <div className="d-none d-sm-block mb-2">
                            {this.state.ProjectData.ProjectName}
                          </div>
                          <p>
                            {" "}
                            <b>Client</b>: {this.state.ProjectData?.ClientName}
                          </p>
                          {Cookies.get("Role") === "SuperAdmin" ? (
                            <button
                              type="button"
                              className="detail_edit"
                              onClick={() => {
                                this.setState(
                                  {
                                    AddProjectField: {
                                      ProjectID:
                                        this.state.ProjectData.ProjectID,
                                      ProjectName:
                                        this.state.ProjectData.ProjectName,
                                      StartDate: new Date(
                                        this.state.ProjectData.StartDate
                                      ),
                                      EndDate: new Date(
                                        this.state.ProjectData.EndData
                                      ),
                                      TaskOwner:
                                        this.state.ProjectData.ProjectOwner,
                                      ProjectType:
                                        this.state.ProjectData.ProjectType,
                                      Status: this.state.ProjectData.Status,
                                      ProjectDescription:
                                        this.state.ProjectData
                                          .ProjectDescription,
                                      ClientID:
                                        this.state.ProjectData?.ClientID,
                                    },
                                  },
                                  () => {
                                    this.OpenUpdateProjectModal();
                                  }
                                );
                              }}
                            >
                              <i className="fa fa-pencil"></i>
                            </button>
                          ) : (
                            ""
                          )}
                        </h4>
                        <table
                          class="table table-responsive-sm border-0"
                          id="ems_project_details_table"
                        >
                          <thead>
                            <tr>
                              <th className="pl-0">Start Date</th>
                              <th>End Date</th>
                              <th>Project Type</th>
                              <th>Status</th>
                            </tr>
                          </thead>
                          <tbody>
                            <tr>
                              <td className="pl-0">
                                {moment(
                                  new Date(this.state.ProjectData.StartDate)
                                ).format("DD/MM/YYYY")}
                              </td>
                              <td>
                                {moment(
                                  new Date(this.state.ProjectData.EndData)
                                ).format("DD/MM/YYYY")}
                              </td>
                              <td>{this.state.ProjectData.ProjectType}</td>
                              <td>
                                {this.state.ProjectData.Status == 1
                                  ? "Active"
                                  : "In-Active"}
                              </td>
                            </tr>
                          </tbody>
                        </table>
                        <p className="mb-0">
                          <b>Project Description</b>
                        </p>
                        <p>{this.state.ProjectData.ProjectDescription}</p>
                        <div class="row align-items-top">
                          <div class="col-md-7">
                            <p
                              className="pb-1"
                              style={{ marginBottom: "-5px" }}
                            >
                              Monthly Budget For
                              <div
                                class="custom_select_latest_outer"
                                id="ems_calendar_sched"
                                style={{ float: "inherit" }}
                              >
                                <Dropdown>
                                  <Dropdown.Toggle
                                    id="dropdown-basic"
                                    className="btn-black mr-1 project_list_dropdwon"
                                  >
                                    {this.state.monthBudgetSelect}
                                  </Dropdown.Toggle>

                                  <Dropdown.Menu className="align_top dashboard_dropdown">
                                    <Dropdown.Item>
                                      <div
                                        className="custom_select_latest"
                                        id="scrollableDiv"
                                      >
                                        {/* <option value="select" selected disabled hidden>
                                          {this.state.monthBudgetSelect}
                                        </option> */}
                                        <option
                                          value="All"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          All
                                        </option>
                                        <option
                                          value="January"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          January
                                        </option>
                                        <option
                                          value="February"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          February
                                        </option>
                                        <option
                                          value="March"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          March
                                        </option>
                                        <option
                                          value="April"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          April
                                        </option>
                                        <option
                                          value="May"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          May
                                        </option>
                                        <option
                                          value="June"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          June
                                        </option>
                                        <option
                                          value="July"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          July
                                        </option>
                                        <option
                                          value="August"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          August
                                        </option>
                                        <option
                                          value="September"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          September
                                        </option>
                                        <option
                                          value="October"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          October
                                        </option>
                                        <option
                                          value="November"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          November
                                        </option>
                                        <option
                                          value="December"
                                          onClick={
                                            this.handleMonthBudgetProjectShow
                                          }
                                        >
                                          December
                                        </option>
                                      </div>
                                    </Dropdown.Item>
                                  </Dropdown.Menu>
                                </Dropdown>
                              </div>
                            </p>
                            <b
                              style={{
                                float: "left",
                                lineHeight: "3px",
                              }}
                            >
                              {
                                this.state.monthBudget[
                                  this.state.monthBudgetSelect
                                ]
                              }
                            </b>
                          </div>

                          {/* <div
                              className="dashboard_select auto_select pt-3"
                              style={{
                                float: "none",
                              }}
                            >
                              <select
                                name="select"
                                id="select"
                                class="form-control px-2"
                              >
                                <option value="" selected disabled hidden>
                                  {this.state.monthBudgetSelect}
                                </option>

                                <option
                                  value="January"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  January
                                </option>
                                <option
                                  value="February"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  February
                                </option>
                                <option
                                  value="March"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  March
                                </option>
                                <option
                                  value="April"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  April
                                </option>
                                <option
                                  value="May"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  May
                                </option>
                                <option
                                  value="June"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  June
                                </option>
                                <option
                                  value="July"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  July
                                </option>
                                <option
                                  value="August"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  August
                                </option>
                                <option
                                  value="September"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  September
                                </option>
                                <option
                                  value="October"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  October
                                </option>
                                <option
                                  value="November"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  November
                                </option>
                                <option
                                  value="December"
                                  onClick={this.handleMonthBudgetProjectShow}
                                >
                                  December
                                </option>
                              </select>
                            </div> */}

                          <div class="col-md-4 offset-1 ems_progress_bar">
                            <p className="mb-0">
                              <b>Progress</b>
                            </p>
                            <div
                              className="progress"
                              style={{
                                position: "relative",
                                justifyContent: "center",
                                boxShadow: " 3px 4px 10px #888888",
                              }}
                            >
                              <div
                                className={
                                  this.state.monthBudgetSelect == "All"
                                    ? "progress-bar bg-gradient"
                                    : progressbarStyle
                                }
                                role="progressbar"
                                style={{
                                  width: `${this.state.ProjectData.Progress}%`,
                                }}
                                aria-valuenow={`${this.state.ProjectData.Progress}`}
                                aria-valuemin="0"
                                aria-valuemax="100"
                              >
                                <span
                                  style={{
                                    zIndex: "1",
                                    width: "fit-content",
                                    display: "block",
                                    margin: "0 auto",
                                    color: "black",
                                  }}
                                >
                                  {this.state.monthBudgetSelect == "All"
                                    ? this.state.ProjectData.TotalHours + "hrs"
                                    : this.state.ProjectData.Progress + "%"}
                                </span>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-sm-12">
                <div class="card">
                  <div class="card-body ">
                    <h3 class="font-weight-semibold mb-3 m_font_heading">
                      Time spent on each task
                    </h3>
                    <div
                      class="text-left detail_toggales"
                      style={{ position: "static", height: "50px" }}
                    >
                      <span
                        className={`HoursDays ${this.state.DayHourToggleBar.dayStyle}`}
                        onClick={() => {
                          this.setState({
                            DayHourToggleBar: {
                              dayStyle: "btn_underline",
                              hourStyle: "",
                            },
                          });
                          this.setState({ GraphdataFormat: "Days" });
                        }}
                      >
                        Days
                      </span>
                      <span
                        className={`HoursDays ${this.state.DayHourToggleBar.hourStyle}`}
                        onClick={() => {
                          this.setState({
                            DayHourToggleBar: {
                              dayStyle: "",
                              hourStyle: "btn_underline",
                            },
                          });
                          this.setState({ GraphdataFormat: "Hours" });
                        }}
                      >
                        Hours
                      </span>
                    </div>
                    <div
                      style={{
                        position: "relative",
                        height: "400PX",
                      }}
                    >
                      <Bar
                        ref={(chart) => (this.chart = chart)}
                        data={Barstate}
                        onElementsClick={(element) => {
                          const chart = element[0]?._chart;
                          const xLabel =
                            chart?.data?.labels[element[0]?._index];
                          if ((this.state.GraphdataFormat = "Days")) {
                            for (
                              var i = 0;
                              backgroundColorDay.length > i;
                              i++
                            ) {
                              backgroundColorDay[i] = "#474747";
                            }
                          } else {
                            for (
                              var i = 0;
                              backgroundColorHour.length > i;
                              i++
                            ) {
                              backgroundColorHour[i] = "#8d9196";
                            }
                          }
                          this.setState({ BarColorIndex: element[0]?._index });
                          this.chart.chartInstance.update();
                          this.setState({ BarLabelOnClick: xLabel }, () => {
                            this.LoadSubTaskTimeSpent();
                            this.LoadSubTaskTimeOwnerSpent();
                          });
                        }}
                        options={{
                          tooltips: {
                            displayColors: false,
                          },
                          responsive: true,
                          maintainAspectRatio: false,
                          scales: {
                            xAxes: [
                              {
                                categoryPercentage: 0.2,
                                barPercentage: 1,
                                ticks: {
                                  callback: function (label, index, labels) {
                                    //wrap graph label on x-axis
                                    if (Dropdownvalue == "All") {
                                      return label;
                                    } else {
                                      if (/\s/.test(label)) {
                                        return label.split(" ");
                                      } else {
                                        return label;
                                      }
                                    }
                                  },
                                  mirror: true,
                                },
                                gridLines: {
                                  color: "rgba(0, 0, 0, 0)",
                                },
                              },
                            ],
                            yAxes: [
                              {
                                gridLines: {
                                  color: "rgba(0, 0, 0, 0)",
                                },
                                ticks: {
                                  beginAtZero: true,
                                },
                              },
                            ],
                          },
                          legend: {
                            display: false,
                          },

                          /*  onClick: function (e) {
                          
                          var element = this.getElementAtEvent(e);
                          // changes only the color of the active object
                          for (var i = 0; backgroundColorDay.length > i; i++) {
                            backgroundColorDay[i] = "#474747";
                          }
                          backgroundColorDay[element[0]._index] = "#0080FF";
                          this.update();
                          console.log();
                        }, */
                        }}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-sm-12">
                <div class="card" style={{ border: "none", boxShadow: "none" }}>
                  <div class="card-body text-center">
                    <div class="row">
                      <div class="col-sm-12">
                        <h3 class="font-weight-semibold mb-3 m_font_heading">
                          <div class="text-left detail_toggales">
                            <span
                              className={`HoursDays ${this.state.DayHourToggleDoughnut.dayStyle}`}
                              onClick={() => {
                                this.setState({
                                  DayHourToggleDoughnut: {
                                    dayStyle: "btn_underline",
                                    hourStyle: "",
                                  },
                                });

                                this.setState({
                                  DoughnutState: {
                                    ...this.state.DoughnutState,
                                    DoughnutDays:
                                      this.state.DoughnutState.DoughnutDataDays
                                        .length > 1
                                        ? parseFloat(
                                            this.state.DoughnutState.DoughnutDataDays?.reduce(
                                              (a, v) => (a = a + v),
                                              0
                                            ).toFixed(2)
                                          )
                                        : this.state.DoughnutState
                                            .DoughnutDataDays[0],
                                    DoughnutName:
                                      this.state.DoughnutState.DoughnutLabels
                                        .length > 1
                                        ? "All"
                                        : this.state.DoughnutState
                                            .DoughnutLabels[0],
                                  },
                                });
                                this.setState({
                                  GraphdataFormatDoughnut: "Days",
                                });
                              }}
                            >
                              Days
                            </span>
                            <span
                              className={`HoursDays ${this.state.DayHourToggleDoughnut.hourStyle}`}
                              onClick={() => {
                                this.setState({
                                  DayHourToggleDoughnut: {
                                    dayStyle: "",
                                    hourStyle: "btn_underline",
                                  },
                                });
                                this.setState({
                                  DoughnutState: {
                                    ...this.state.DoughnutState,
                                    DoughnutDays:
                                      this.state.DoughnutState.DoughnutDataHours
                                        .length > 1
                                        ? parseFloat(
                                            this.state.DoughnutState.DoughnutDataHours?.reduce(
                                              (a, v) => (a = a + v),
                                              0
                                            ).toFixed(2)
                                          )
                                        : this.state.DoughnutState
                                            .DoughnutDataHours[0],
                                    DoughnutName:
                                      this.state.DoughnutState.DoughnutLabels
                                        .length > 1
                                        ? "All"
                                        : this.state.DoughnutState
                                            .DoughnutLabels[0],
                                  },
                                });
                                this.setState({
                                  GraphdataFormatDoughnut: "Hours",
                                });
                              }}
                            >
                              Hours
                            </span>
                          </div>
                          <button
                            type="button"
                            className="detail_edit"
                            onClick={() => {
                              this.setState({ BarLabelOnClick: "" }, () => {
                                this.LoadSubTaskTimeSpent();
                                this.LoadSubTaskTimeOwnerSpent();
                              });
                            }}
                          >
                            <i className="fa fa-refresh"></i>
                          </button>
                          Sub Task Details
                        </h3>
                        <div
                          className="chart_outer"
                          style={{
                            position: "relative",
                          }}
                        >
                          <div id="inner_deta">
                            <span>{this.state.DoughnutState.DoughnutName}</span>
                            <br />
                            <span style={{ fontSize: "12px" }}>
                              {this.state.GraphdataFormatDoughnut == "Days"
                                ? "Days:"
                                : "Hours:"}
                            </span>

                            <p style={{ fontSize: "12px" }}>
                              {this.state.GraphdataFormatDoughnut == "Days"
                                ? `${
                                    this.state.DoughnutState.DoughnutDays ==
                                    undefined
                                      ? 0
                                      : this.state.DoughnutState.DoughnutDays
                                  } Days`
                                : `${
                                    this.state.DoughnutState.DoughnutDays ==
                                    undefined
                                      ? 0
                                      : this.state.DoughnutState.DoughnutDays
                                  } Hours`}
                            </p>
                          </div>

                          <Doughnut
                            data={Doughnutstate}
                            options={{
                              responsive: true,
                              maintainAspectRatio: false,
                              cutoutPercentage: 80,
                              legend: {
                                display: false,
                              },
                              centerText: {
                                data: this.state.DoughnutState.DoughnutDataDays,
                                display: false,
                                text: "20%",
                                zIndex: 999,
                              },
                              tooltips: {
                                // backgroundColor: "transparent",
                                // rtl: true,
                                // textColor: "#000",
                                displayColors: false,
                                callbacks: {
                                  label: function (tooltipItem, data) {
                                    //get the concerned dataset
                                    var dataset =
                                      data.datasets[tooltipItem.datasetIndex];
                                    //to get the label
                                    var dataLabel =
                                      data.labels[tooltipItem.index];
                                    //get the current items value
                                    var currentValue =
                                      dataset.data[tooltipItem.index];

                                    this.setState({
                                      DoughnutState: {
                                        ...this.state.DoughnutState,
                                        DoughnutDays: currentValue,
                                        DoughnutName: dataLabel,
                                      },
                                    });

                                    return `${dataLabel} : ${currentValue}`;
                                  }.bind(this),
                                },
                              },
                            }}
                          />
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-sm-12">
                <div class="card card_half_width">
                  <div class="card-body">
                    <table class="table table-responsive-sm table-striped table-bordered">
                      <thead>
                        <tr>
                          <th>Days</th>
                          <th>Sub Task</th>
                          <th>Contributor</th>
                          <th>Action</th>
                        </tr>
                      </thead>
                      <tbody>{TaskOwnerNameList}</tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
            <BlogContributionPopup
              Title={this.state.ShowTaskOwnerTitle}
              ShowTaskOwner={this.state.ShowTaskOwnerName}
              BlogContributionShow={this.state.blogContributionShow}
              closeBlogContribution={() => {
                this.setState({ blogContributionShow: false });
              }}
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
              //FieldErrorStyle={this.state.errorStyle}
              ErrorStyleRemovedAddProject={this.errorStyleRemovedAddProject}
              CurrentBudgetYear={this.state.currentBudgetYear}
              IncrementYear={this.incrementYear}
              DecrementYear={this.decrementYear}
              ShowData={this.state.AddProjectField}
              HandelErrorRemove={this.HandelErrorRemove}
              errorStyle={this.state.errorStyle}
            />

            <PopUpModal
              Title={this.state.Title}
              errorMsgs={this.state.errorMsg}
              show={this.state.PopUpBit}
              onHide={this.CloseModal}
            />
          </div>
        )}
      </>
    );
  }
}
export default ProjectDetails;
