import React, { Component } from "react";
import axios from "axios";
import moment from "moment";
import { Dropdown, DropdownButton } from "react-bootstrap";
import "react-dropdown/style.css";
import Loader from "../Loader/Loader";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import PopUpModal from "../PopUpModalAdmin/PopUpModal";
import AddSettingModal from "./AddSetting";
import UpdateSettingModal from "./UpdateSetting";
import SearchSetting from "./SearchSetting";
import "./EMSSetting.css";
import TextField from "@material-ui/core/TextField";
import Select from "@material-ui/core/Select";
import InputLabel from "@material-ui/core/InputLabel";
import FormControl from "@material-ui/core/FormControl";
import "../../Material_UI_Inputs/Material_UI_Inputs.css";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "react-crypt-gsm";
class EmsSetting extends Component {
  constructor(props) {
    super(props);
    this.state = {
      SearchedSettingList: [],
      SettingDropdownTitle: "All",
      getTypeData: [],
      SettingTypeData: [],
      PopUpBit: false,
      page: 0,
      rowsPerPage: 10,
      SettingCount: 0,
      pageA: 0,
      rowsPerPageA: 10,
      SettingCountA: 0,
      pageB: 0,
      rowsPerPageB: 10,
      SettingCountB: 0,
      pageC: 0,
      rowsPerPageC: 10,
      SettingCountC: 0,
      lstSettings: null,
      Loading: true,
      AddSettingDate: new Date(),
      SettingName: "",
      SettingValue: "",
      SettingUserName: "",
      SettingType: "",
      SettingDescription: "",
      MsgModel: false,
      Msg: "",
      MsgTitle: "",
      SearchSettingName: "",
      SwitchDisplay: false,
      SearchFilter: false,
      UpdateFilter: false,
      SettingId: null,
      error_class: {
        SettingName: "",
        SettingValue: "",
        SettingUserName: "",
        SettingType: "",
        SettingDescription: "",
      },
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
    this.LoadSettings();
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
  switchSettingTypes = (settingName, settingDataa = []) => {
    var settingArray = [];
    var settingData = this.state.getTypeData;

    switch (settingName) {
      case "Setting A":
        settingArray = [];

        this.state.SwitchDisplay === false &&
          settingArray.push({ name: settingName, data: settingData.SettingA });
        // console.log(settingArray[0].data);
        this.setState({ SettingTypeData: settingArray });
        this.setState({ SettingCountA: settingArray[0].data.length });
        break;

      case "Setting B":
        settingArray = [];

        this.state.SwitchDisplay === false &&
          settingArray.push({ name: settingName, data: settingData.SettingB });
        this.setState({ SettingTypeData: settingArray });
        this.setState({ SettingCountB: settingArray[0].data.length });
        break;

      case "Setting C":
        settingArray = [];

        this.state.SwitchDisplay === false &&
          settingArray.push({ name: settingName, data: settingData.SettingC });
        this.setState({ SettingTypeData: settingArray });
        this.setState({ SettingCountC: settingArray[0].data.length });
        break;

      default:
        settingArray = [];
        settingArray.push({ name: "Setting A", data: settingData.SettingA });
        settingArray.push({ name: "Setting B", data: settingData.SettingB });
        settingArray.push({ name: "Setting C", data: settingData.SettingC });

        this.setState({ SettingTypeData: settingArray });
        this.setState({ SettingCountA: settingArray[0].data.length });
        this.setState({ SettingCountB: settingArray[1].data.length });
        this.setState({ SettingCountC: settingArray[2].data.length });

      //this.ClearSearchFilter();
      // this.setState({SwitchDisplay:true});
    }
  };

  LoadSettings = (settingName = "") => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Setting/GetSetting`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: "",
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ getTypeData: res.data });

        this.switchSettingTypes("All");

        this.setState({ lstSettings: res.data.Result }, () => {
          this.setState({
            SettingCount: Object.keys(this.state.lstSettings).length,
          });
          this.setState({ SearchedSettingList: this.state.lstSettings });
          // console.log(this.state.lstSettings);
          this.setState({ Loading: false });
          this.CloseModal();
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  SearchSettings = () => {
    try {
      if (this.state.SearchSettingName !== "") {
        var searchedArray = [];
        var searchTerm = this.state.SearchSettingName;
        var resultData = this.state.SearchedSettingList;

        resultData.filter((val) => {
          if (
            val.Name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            val.Value.toLowerCase().includes(searchTerm.toLowerCase()) ||
            val.LastModifiedBy.toLowerCase().includes(
              searchTerm.toLowerCase()
            ) ||
            val.SettingDescription.toLowerCase().includes(
              searchTerm.toLowerCase()
            ) ||
            val.UserName.toLowerCase().includes(searchTerm.toLowerCase())
          ) {
            return searchedArray.push(val);
          }
        });

        if (searchedArray.length > 0) {
          this.setState({ lstSettings: searchedArray }, () => {
            this.setState({
              SettingCount: searchedArray.length,
            });

            this.setState({ SwitchDisplay: true });
            this.setState({ SettingDropdownTitle: "Select Type" });
          });
        } else {
          this.setState({ lstSettings: null });
        }

        this.setState({ Loading: false });
        this.CloseModal();

        // const SearchSettings = {
        //   SettingName: this.state.SearchSettingName,
        // };
        // axios({
        //   method: "post",
        //   url: `${process.env.REACT_APP_BASE_URL}Setting/GetSetting`,
        //   headers: {
        //     Authorization: "Bearer " + localStorage.getItem("access_token"),
        //   },
        //   data: SearchSettings,
        // }).then((res) => {

        //   if (res.data.Result !== "") {

        //     this.setState({ lstSettings: res.data.Result }, () => {
        //       this.setState({
        //         SettingCount: Object.keys(this.state.lstSettings).length,
        //       });

        //       this.setState({SwitchDisplay:true});
        //       this.setState({SettingDropdownTitle:'Select Type'});
        //     });
        //   } else {
        //     this.setState({ lstSettings: null });
        //   }

        //   this.setState({ Loading: false });
        //   this.CloseModal();
        // });
      } else {
        this.setState({ MsgModel: true });
        this.setState({ Msg: "Search Setting Field Cannot be Empty!!!" });
        this.setState({ MsgTitle: "Error" });
      }
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  validatePopUp = (type) => {
    var checkError = false;
    if (this.state.SettingName == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          SettingName: "input_error",
        },
      }));
    }
    if (this.state.SettingValue == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          SettingValue: "input_error",
        },
      }));
    }
    // if (this.state.SettingUserName == "") {
    //   checkError = true;
    //   this.setState((prevState) => ({
    //     error_class: {
    //       ...prevState.error_class,
    //       SettingUserName: "input_error",
    //     },
    //   }));
    // }
    if (this.state.SettingType == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          SettingType: "input_error",
        },
      }));
    }

    if (this.state.SettingDescription == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          SettingDescription: "input_error",
        },
      }));
    }

    if (type === "addSetting") {
      if (!checkError) {
        this.AddSettings();
      }
    }

    if (type === "updateSetting") {
      if (!checkError) {
        this.UpdateSetting();
      }
    }
  };

  HandelErrorRemove = (name) => {
    if (name == "SettingName") {
      if (this.state.SettingName == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            SettingName: "",
          },
        }));
      }
    }
    if (name == "SettingName") {
      if (this.state.SettingName == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            SettingName: "",
          },
        }));
      }
    }
    if (name == "SettingValue") {
      if (this.state.SettingValue == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            SettingValue: "",
          },
        }));
      }
    }
    // if (name == "SettingUserName") {
    //   if (this.state.SettingUserName == "") {
    //     this.setState((prevState) => ({
    //       error_class: {
    //         ...prevState.error_class,
    //         SettingUserName: "",
    //       },
    //     }));
    //   }
    // }
    if (name == "SettingType") {
      if (this.state.SettingType == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            SettingType: "",
          },
        }));
      }
    }
    if (name == "SettingDescription") {
      if (this.state.SettingDescription == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            SettingDescription: "",
          },
        }));
      }
    }
  };

  AddSettings = () => {
    this.setState({ SwitchDisplay: false });
    this.setState({ SettingDropdownTitle: "All" });

    console.log();

    try {
      const AddSettings = {
        Id: 0,
        Name: this.state.SettingName,
        Value: this.state.SettingValue,
        UserName: Cookies.get("UserName"),
        SettingDescription: this.state.SettingDescription,
        SettingType: this.state.SettingType,
        Mode: false,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Setting/InsertSetting`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AddSettings,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == "200") {
          this.setState({ MsgModel: true });
          this.setState({ Msg: "Setting Saved Sucessfully " });
          this.setState({ MsgTitle: "Sucess" });
        } else {
          this.setState({ MsgModel: true });
          this.setState({ Msg: "Error in Saving Setting" });
          this.setState({ MsgTitle: "Failed" });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };

  UpdateSetting = () => {
    this.setState({ SwitchDisplay: false });
    this.setState({ SettingDropdownTitle: "All" });

    try {
      const UpdateSettings = {
        Id: this.state.SettingId,
        Name: this.state.SettingName,
        Value: this.state.SettingValue,
        UserName: Cookies.get("UserName"),
        SettingDescription: this.state.SettingDescription,
        SettingType: this.state.SettingType,
        Mode: false,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Setting/InsertSetting`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UpdateSettings,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == "200") {
          this.setState({ MsgModel: true });
          this.setState({ Msg: "Setting Update Sucessfully " });
          this.setState({ MsgTitle: "Sucess" });
        } else {
          this.setState({ MsgModel: true });
          this.setState({ Msg: "Error in Updating Setting" });
          this.setState({ MsgTitle: "Failed" });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  DeleteSetting = () => {
    this.setState({ SwitchDisplay: false });
    this.setState({ SettingDropdownTitle: "All" });

    try {
      const DeleteSettings = {
        Id: this.state.SettingId,
        Mode: true,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Setting/InsertSetting`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DeleteSettings,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == "200") {
          this.setState({ MsgModel: true });
          this.setState({ Msg: "Setting Delete Sucessfully " });
          this.setState({ MsgTitle: "Sucess" });
        } else {
          this.setState({ MsgModel: true });
          this.setState({ Msg: "Error in Deleting Setting" });
          this.setState({ MsgTitle: "Failed" });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  OpenAddSettingBox = () => {
    this.setState({ SettingName: "" });
    this.setState({ SettingUserName: "" });
    this.setState({ SettingDescription: "" });
    this.setState({ SettingType: "" });
    this.setState({ SettingValue: "" });
    this.setState({ AddSettingDate: new Date() });
    this.setState({ PopUpBit: true });
  };
  OpenUpdateSetting = () => {
    this.setState({ UpdateFilter: true });
  };

  CloseModal = () => {
    this.setState({
      error_class: {
        SettingName: "",
        SettingValue: "",
        SettingUserName: "",
        SettingDescription: "",
        SettingType: "",
      },
    });
    if (this.state.MsgTitle == "Sucess") {
      this.setState({ MsgTitle: false }, () => {
        this.componentDidMount();
      });
    }
    this.setState({ PopUpBit: false });
    this.setState({ MsgModel: false });
    this.setState({ SearchFilter: false });
    this.setState({ UpdateFilter: false });
  };
  HandleDateChange = (date) => {
    this.setState({ AddSettingDate: date });
  };
  handleAddSettingInput = (e) => {
    try {
      /*  this.setState({
        error_class: {
          SettingName: "",
          SettingValue: "",
          SettingUserName: "",
        },
      }); */
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
  handleSearchSettingInput = (e) => {
    this.setState({ SearchSettingName: e.target.value });
  };
  SearchSettingFilter = () => {
    this.setState({ SearchFilter: true });
  };
  ClearSearchFilter = () => {
    this.setState({ SearchSettingName: "" }, () => {
      this.setState({ SwitchDisplay: false });
      this.setState({ SettingDropdownTitle: "All" });
      this.LoadSettings();
    });
  };

  getSettingTypeData = (name, type, dataArray = []) => {
    if (type === "populateData") {
      if (name === "Setting A") {
        let data = dataArray.slice(
          this.state.pageA * this.state.rowsPerPageA,
          this.state.pageA * this.state.rowsPerPageA + this.state.rowsPerPageA
        );
        return data;
      } else if (name === "Setting B") {
        let data = dataArray.slice(
          this.state.pageB * this.state.rowsPerPageB,
          this.state.pageB * this.state.rowsPerPageB + this.state.rowsPerPageB
        );
        return data;
      } else if (name === "Setting C") {
        let data = dataArray.slice(
          this.state.pageC * this.state.rowsPerPageC,
          this.state.pageC * this.state.rowsPerPageC + this.state.rowsPerPageC
        );
        return data;
      }
    }

    if (type === "count") {
      if (name === "Setting A") {
        return this.state.SettingCountA;
      } else if (name === "Setting B") {
        return this.state.SettingCountB;
      } else if (name === "Setting C") {
        return this.state.SettingCountC;
      }
    }

    if (type === "page") {
      if (name === "Setting A") {
        return this.state.pageA;
      } else if (name === "Setting B") {
        return this.state.pageB;
      } else if (name === "Setting C") {
        return this.state.pageC;
      }
    }

    if (type === "rowsPerPage") {
      if (name === "Setting A") {
        return this.state.rowsPerPageA;
      } else if (name === "Setting B") {
        return this.state.rowsPerPageB;
      } else if (name === "Setting C") {
        return this.state.rowsPerPageC;
      }
    }

    if (type === "changePage") {
      if (name === "Setting A") {
        return (event, newPage) => {
          this.setState({ pageA: newPage });
        };
      } else if (name === "Setting B") {
        return (event, newPage) => {
          this.setState({ pageB: newPage });
        };
      } else if (name === "Setting C") {
        return (event, newPage) => {
          this.setState({ pageC: newPage });
        };
      }
    }

    if (type === "changeRowsPerPage") {
      if (name === "Setting A") {
        return (event) => {
          this.setState({ rowsPerPageA: parseInt(event.target.value, 10) });
          this.setState({ pageA: 0 });
        };
      } else if (name === "Setting B") {
        return (event) => {
          this.setState({ rowsPerPageB: parseInt(event.target.value, 10) });
          this.setState({ pageB: 0 });
        };
      } else if (name === "Setting C") {
        return (event) => {
          this.setState({ rowsPerPageC: parseInt(event.target.value, 10) });
          this.setState({ pageC: 0 });
        };
      }
    }
  };

  settingTypeDropdown = (title) => {
    var name = title;

    this.setState({ SettingDropdownTitle: name });

    this.setState({ SwitchDisplay: false }, () => {
      this.switchSettingTypes(name);
    });
  };

  render() {
    return (
      <div ref="component">
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div className="row rpt_mbl_header mb-3">
                <div
                  className="col-sm-4 text-left"
                  style={{ marginBottom: "10px" }}
                >
                  <h3 className="m_font_heading">Settings</h3>
                </div>

                <div className="col-sm-8 d-flex justify-content-end ems_setting_actions">
                  <button
                    onClick={this.SearchSettingFilter}
                    type="button"
                    className="btn-black"
                    data-toggle="modal"
                    data-target="#reportsFilter"
                    style={{
                      float: "right",
                      marginLeft: "10px",
                      marginBottom: "10px",
                    }}
                  >
                    Setting Filter <i className="menu-icon fa fa-plus"></i>
                  </button>

                  <button
                    onClick={this.OpenAddSettingBox}
                    type="button"
                    className="btn-black mr-1"
                    data-toggle="modal"
                    data-target="#reportsFilter"
                    style={{
                      float: "right",
                      marginLeft: "10px",
                      marginBottom: "10px",
                    }}
                  >
                    Add Setting
                  </button>

                  <DropdownButton
                    variant="black"
                    id="dropdown-basic-button"
                    className="settingMenu"
                    title={this.state.SettingDropdownTitle}
                    style={{ marginLeft: "6px" }}
                  >
                    <Dropdown.Item
                      onClick={() => this.settingTypeDropdown("Setting A")}
                    >
                      Setting A
                    </Dropdown.Item>
                    <Dropdown.Item
                      onClick={() => this.settingTypeDropdown("Setting B")}
                    >
                      Setting B
                    </Dropdown.Item>
                    <Dropdown.Item
                      onClick={() => this.settingTypeDropdown("Setting C")}
                    >
                      Setting C
                    </Dropdown.Item>
                    <Dropdown.Item
                      onClick={() => this.settingTypeDropdown("All")}
                    >
                      All
                    </Dropdown.Item>
                  </DropdownButton>
                </div>
              </div>

              {this.state.SwitchDisplay === true ? (
                <div className="card">
                  {this.state.Loading ? (
                    <Loader />
                  ) : (
                    <div className="card-body" style={{ paddingTop: "10px" }}>
                      <TableContainer
                        className="table-responsive"
                        component={"div"}
                      >
                        <Table className="table">
                          <TableHead>
                            <TableRow>
                              <TableCell>Setting Name</TableCell>
                              <TableCell>Value</TableCell>
                              <TableCell>Creation Date</TableCell>
                              <TableCell>Created By</TableCell>
                              <TableCell>Modified Date</TableCell>
                              <TableCell>Last Modified By</TableCell>
                              <TableCell>Description</TableCell>
                              {/* <TableCell>Action</TableCell> */}
                            </TableRow>
                          </TableHead>
                          <TableBody>
                            {
                              this.state.lstSettings != null
                                ? this.state.lstSettings
                                    .slice(
                                      this.state.page * this.state.rowsPerPage,
                                      this.state.page * this.state.rowsPerPage +
                                        this.state.rowsPerPage
                                    )
                                    .map((row, index) =>
                                      row.Project === "TOTAL" ? (
                                        <></>
                                      ) : (
                                        <TableRow
                                          key={index}
                                          style={{ cursor: "pointer" }}
                                          onClick={() => {
                                            this.setState({
                                              SettingId: row.Id,
                                            });
                                            this.setState({
                                              SettingName: row.Name,
                                            });
                                            this.setState({
                                              SettingUserName:
                                                row.LastModifiedBy,
                                            });
                                            this.setState({
                                              SettingValue: row.Value,
                                            });
                                            this.setState({
                                              SettingType: row.SettingType,
                                            });
                                            this.setState({
                                              SettingDescription:
                                                row.SettingDescription,
                                            });
                                            this.setState(
                                              {
                                                AddSettingDate: new Date(
                                                  row.CreationDate
                                                ),
                                              },
                                              () => {
                                                this.OpenUpdateSetting();
                                              }
                                            );
                                          }}
                                        >
                                          <TableCell>{row.Name}</TableCell>
                                          <TableCell
                                            style={{ wordBreak: "break-all" }}
                                          >
                                            {row.Value}
                                          </TableCell>
                                          <TableCell>
                                            {row.CreationDate}
                                          </TableCell>
                                          <TableCell>{row.UserName}</TableCell>
                                          <TableCell>
                                            {row.ModificationDate}
                                          </TableCell>
                                          <TableCell>
                                            {row.LastModifiedBy}
                                          </TableCell>
                                          <TableCell>
                                            {row.SettingDescription}
                                          </TableCell>
                                          {/* <TableCell>
                                          <i
                                            className="fa fa-pencil-square-o"
                                            aria-hidden="true"
                                            style={{
                                              cursor: "pointer",
                                              fontSize: "18px",
                                            }}
                                            onClick={() => {
                                              this.setState({
                                                SettingId: row.Id,
                                              });
                                              this.setState({
                                                SettingName: row.Name,
                                              });
                                              this.setState({
                                                SettingUserName: row.UserName,
                                              });
                                              this.setState({
                                                SettingValue: row.Value,
                                              });
                                              this.setState({
                                                SettingType: row.SettingType,
                                              });
                                              this.setState({
                                                SettingDescription: row.SettingDescription
                                              })
                                              this.setState(
                                                {
                                                  AddSettingDate: new Date(
                                                    row.CreationDate
                                                  ),
                                                },
                                                () => {
                                                  this.OpenUpdateSetting();
                                                }
                                              );
                                            }}
                                          ></i>
                                        </TableCell> */}
                                        </TableRow>
                                      )
                                    )
                                : null
                              //<Loader />
                            }
                          </TableBody>
                        </Table>
                      </TableContainer>
                      <TablePagination
                        component="div"
                        count={this.state.SettingCount}
                        page={this.state.page}
                        onChangePage={this.handleChangePage}
                        rowsPerPage={this.state.rowsPerPage}
                        onChangeRowsPerPage={this.handleChangeRowsPerPage}
                      />
                    </div>
                  )}
                </div>
              ) : this.state.Loading ? (
                <Loader />
              ) : (
                this.state.SettingTypeData != null &&
                this.state.SettingTypeData.map((row, index) => (
                  <>
                    <div className="card mb-5" key={index}>
                      <div className="card-header">
                        <div className="row">
                          <div className="col-sm-6">
                            <h5
                              className=""
                              style={{ marginLeft: "5px", marginTop: "9px" }}
                            >
                              {row.name}
                            </h5>
                          </div>
                        </div>
                      </div>

                      <div className="card-body" style={{ paddingTop: "0px" }}>
                        <TableContainer
                          className="table-responsive"
                          component={"div"}
                        >
                          <Table className="table ems_setting_table_data">
                            <TableHead>
                              <TableRow>
                                <TableCell>Setting Name</TableCell>
                                <TableCell>Value</TableCell>
                                <TableCell>Creation Date</TableCell>
                                <TableCell>Created By</TableCell>
                                <TableCell>Modified Date</TableCell>
                                <TableCell>Last Modified By</TableCell>
                                <TableCell>Description</TableCell>
                                {/* <TableCell>Action</TableCell> */}
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {
                                row.data != null
                                  ? this.getSettingTypeData(
                                      row.name,
                                      "populateData",
                                      row.data
                                    ).map((row1, index1) => (
                                      <TableRow
                                        key={index1}
                                        style={{ cursor: "pointer" }}
                                        onClick={() => {
                                          this.setState({
                                            SettingId: row1.Id,
                                          });
                                          this.setState({
                                            SettingName: row1.Name,
                                          });
                                          this.setState({
                                            SettingUserName:
                                              row1.LastModifiedBy,
                                          });
                                          this.setState({
                                            SettingValue: row1.Value,
                                          });
                                          this.setState({
                                            SettingType: row1.SettingType,
                                          });
                                          this.setState({
                                            SettingDescription:
                                              row1.SettingDescription,
                                          });
                                          this.setState(
                                            {
                                              AddSettingDate: new Date(
                                                row1.CreationDate
                                              ),
                                            },
                                            () => {
                                              this.OpenUpdateSetting();
                                            }
                                          );
                                        }}
                                      >
                                        <TableCell>{row1.Name}</TableCell>
                                        <TableCell
                                          style={{ wordBreak: "break-all" }}
                                        >
                                          {row1.Value}
                                        </TableCell>
                                        <TableCell>
                                          {row1.CreationDate}
                                        </TableCell>
                                        <TableCell>{row1.UserName}</TableCell>
                                        <TableCell>
                                          {row1.ModificationDate == "01/01/1900"
                                            ? ""
                                            : row1.ModificationDate}
                                        </TableCell>
                                        <TableCell>
                                          {row1.LastModifiedBy}
                                        </TableCell>
                                        <TableCell>
                                          {row1.SettingDescription}
                                        </TableCell>
                                        {/* <TableCell>
                                                    <i
                                                      className="fa fa-pencil-square-o"
                                                      aria-hidden="true"
                                                      style={{
                                                        cursor: "pointer",
                                                        fontSize: "18px",
                                                      }}
                                                      onClick={() => {
                                                        this.setState({
                                                          SettingId: row1.Id,
                                                        });
                                                        this.setState({
                                                          SettingName: row1.Name,
                                                        });
                                                        this.setState({
                                                          SettingUserName: row1.UserName,
                                                        });
                                                        this.setState({
                                                          SettingValue: row1.Value,
                                                        });
                                                        this.setState({
                                                          SettingType: row1.SettingType,
                                                        });
                                                        this.setState({
                                                          SettingDescription: row1.SettingDescription
                                                        })
                                                        this.setState(
                                                          {
                                                            AddSettingDate: new Date(
                                                              row1.CreationDate
                                                            ),
                                                          },
                                                          () => {
                                                            this.OpenUpdateSetting();
                                                          }
                                                        );
                                                      }}
                                                    ></i>
                                                  </TableCell> */}
                                      </TableRow>
                                    ))
                                  : null
                                //<Loader />
                              }
                            </TableBody>
                          </Table>
                          <TablePagination
                            component="div"
                            count={this.getSettingTypeData(row.name, "count")}
                            page={this.getSettingTypeData(row.name, "page")}
                            onChangePage={this.getSettingTypeData(
                              row.name,
                              "changePage"
                            )}
                            rowsPerPage={this.getSettingTypeData(
                              row.name,
                              "rowsPerPage"
                            )}
                            onChangeRowsPerPage={this.getSettingTypeData(
                              row.name,
                              "changeRowsPerPage"
                            )}
                          />
                        </TableContainer>
                      </div>
                    </div>
                  </>
                ))
              )}
            </div>
          </div>
        </div>

        <PopUpModal
          Title={this.state.MsgTitle}
          errorMsgs={this.state.Msg}
          show={this.state.MsgModel}
          onHide={this.CloseModal}
        />

        <AddSettingModal
          Title="Add Setting"
          show={this.state.PopUpBit}
          Date={this.state.AddSettingDate}
          SettingUserNames={this.state.SettingUserName}
          SettingNames={this.state.SettingName}
          SettingValues={this.state.SettingValue}
          SettingType={this.state.SettingType}
          SettingDescription={this.state.SettingDescription}
          AddSetting={this.validatePopUp}
          handleDate={this.HandleDateChange}
          HandleAddSettingInput={this.handleAddSettingInput}
          onHide={this.CloseModal}
          errorclass={this.state.error_class}
          HandelErrorRemove={this.HandelErrorRemove}
        />

        <UpdateSettingModal
          Title="Update Setting"
          show={this.state.UpdateFilter}
          Date={this.state.AddSettingDate}
          SettingUserNames={this.state.SettingUserName}
          SettingNames={this.state.SettingName}
          SettingValues={this.state.SettingValue}
          SettingType={this.state.SettingType}
          SettingDescription={this.state.SettingDescription}
          DeleteSettings={this.DeleteSetting}
          handleDate={this.HandleDateChange}
          HandleAddSettingInput={this.handleAddSettingInput}
          onHide={this.CloseModal}
          errorclass={this.state.error_class}
          UpdateSetting={this.validatePopUp}
          HandelErrorRemove={this.HandelErrorRemove}
        />
        <SearchSetting
          Title="Search Filter"
          show={this.state.SearchFilter}
          SearchSettingNames={this.state.SearchSettingName}
          SearchSetting={this.SearchSettings}
          handleSearchSettingInput={this.handleSearchSettingInput}
          ClearSearch={this.ClearSearchFilter}
          onHide={this.CloseModal}
        />
      </div>
    );
  }
}

export default EmsSetting;
