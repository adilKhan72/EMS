import React, { Component } from "react";
import axios from "axios";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import Loader from "../Loader/Loader";
import Cookies from "js-cookie";
import ShowNotificationPopUp from "./ShowNotificationPopUp";
import DeleteMultipleNotificationPopUp from "./DeleteMultipleNotificationPopUp";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
class Notification extends Component {
  constructor(props) {
    super(props);
    this.state = {
      NotificationId: null,
      NotficationList: [],
      NotificationLength: 0,
      page: 0,
      rowsPerPage: 10,
      showNotifcation: "",
      showModel: false,
      showDeleteModel: false,
      Loading: true,
      MultiNotificationId: "",
      showMultiDeleteModel: false,
      showNotifcationHeader: null,
      CheckedState: [],
      CheckedAll: [],
    };
  }

  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  CloseModal = () => {
    this.setState({ showModel: false });
    this.setState({ showDeleteModel: false });
    this.setState({ showMultiDeleteModel: false });
  };

  DeleteMultipleNotification = (e) => {
    try {
      var obj = { NotificationID: this.state.MultiNotificationId };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/DeleteNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: obj,
      }).then((response) => {
        if (
          response.data.StatusCode == "200" &&
          response.data.Result.ResponseCode == "1"
        ) {
          this.setState({ MultiNotificationId: "" });
          this.setState({ showMultiDeleteModel: false });
          this.RefreshNotificationGrid();
        } else {
          this.setState({
            showNotifcation: "Error while deleting Notifications",
          });
          this.setState({ showModel: true });
        }
        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };

  RefreshNotificationGrid = () => {
    try {
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/GetNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: NotifyObj,
      }).then((response) => {
        if (response.data.StatusCode == "200") {
          var res = response.data.Result;
          this.setState({ NotficationList: res });
          this.setState({ NotificationLength: response.data.Result.length });

          if (this.props.location.NCFun == undefined) {
          } else {
            this.props.location.NCFun.notificationCounts();
          }
        }

        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };

  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  componentDidMount() {
    try {
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/GetNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: NotifyObj,
      }).then((response) => {
        if (
          response.data.StatusCode == "200" &&
          response.data.Result.length > 0
        ) {
          //console.log(res);
          var res = response.data.Result;
          this.setState({ NotficationList: res });
          this.setState({ NotificationLength: response.data.Result.length });

          if (this.props.location.NCFun == undefined) {
          } else {
            this.props.location.NCFun.UnReadNotififcationlist();
            this.props.location.NCFun.notificationCounts();
          }
        }
        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  }
  ReadNotification = (ID) => {
    try {
      const NotifyObj = {
        UserId: Cookies.get("UserID"),
        NotificationId: ID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Notification/ReadNotification`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: NotifyObj,
      }).then((response) => {
        this.componentDidMount();
        this.setState({ Loading: false });
      });
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  SelectAll = () => {
    var tempArray = [];
    var MultiNotificationID = "";
    if (this.state.NotficationList.length > 0) {
      this.state.NotficationList.map((item) => {
        const tempobj = {
          isTrue: true,
          NotificationID: item.NotificationId,
        };
        tempArray.push(tempobj);
        MultiNotificationID = MultiNotificationID.concat(
          item.NotificationId + ","
        );
      });

      this.setState({
        CheckedState: tempArray,
      });

      this.setState({
        MultiNotificationId: MultiNotificationID,
      });
    }
  };

  render() {
    const CellStyle = {
      maxWidth: "100px",
      overflow: "hidden",
      textOverflow: "ellipsis",
      whiteSpace: "nowrap",
      cursor: "pointer",
      padding: "none",
    };

    return (
      <div>
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div className="card-header">
                <div className="row align-items-center">
                  <div className="col-sm-6" style={{ marginBottom: "20px" }}>
                    <h3 className="m_font_heading">Notifications</h3>
                  </div>
                  {this.state.MultiNotificationId != "" &&
                  this.state.MultiNotificationId != "," ? (
                    <div className="col-sm-12 text-right">
                      <button
                        onClick={this.SelectAll}
                        type="button"
                        className="btn-black"
                        data-toggle="modal"
                        data-target="#reportsFilter"
                        style={{ marginBottom: "20px", marginRight: "10px" }}
                      >
                        Select All
                      </button>
                      <button
                        onClick={() => {
                          this.setState({ showMultiDeleteModel: true });
                          this.setState({
                            showNotifcation:
                              "Are you sure you want to delete the selected notification?",
                          });
                        }}
                        type="button"
                        className="btn-black"
                        data-toggle="modal"
                        data-target="#reportsFilter"
                      >
                        Delete
                        <i
                          className="fa ico-icon  fa-trash-o cursor-pointer"
                          style={{
                            color: "white",
                            marginLeft: "4px",
                            fontSize: "15px",
                          }}
                        ></i>
                      </button>
                    </div>
                  ) : null}
                </div>
              </div>

              <div className="card">
                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div className="card-body notification_listing">
                    <TableContainer component={"div"}>
                      <Table id="bootstrap-data-table">
                        <TableBody>
                          {this.state.NotficationList.length > 0 ? (
                            this.state.NotficationList.slice(
                              this.state.page * this.state.rowsPerPage,
                              this.state.page * this.state.rowsPerPage +
                                this.state.rowsPerPage
                            ).map((item, key) => {
                              return (
                                <TableRow
                                  style={
                                    item.isRead == 0
                                      ? { backgroundColor: "lightgray" }
                                      : { backgroundColor: "white" }
                                  }
                                >
                                  <TableCell width="7%" padding="none">
                                    <FormControlLabel
                                      control={
                                        <Checkbox
                                          style={{
                                            color: "dark grey !important",
                                            marginLeft: "13px",
                                            backgroundColor: "#fff",
                                          }}
                                          onChange={(event) => {
                                            const tempobj = {
                                              isTrue: event.target.checked,
                                              NotificationID:
                                                item.NotificationId,
                                            };
                                            //if it is already exist delete it
                                            if (
                                              this.state.CheckedState.findIndex(
                                                (i) =>
                                                  i.NotificationID ==
                                                  item.NotificationId
                                              ) != -1
                                            ) {
                                              var myArray =
                                                this.state.CheckedState.filter(
                                                  function (obj) {
                                                    return (
                                                      obj.NotificationID !==
                                                      item.NotificationId
                                                    );
                                                  }
                                                );
                                              this.setState({
                                                CheckedState: myArray,
                                              });
                                            } else {
                                              this.setState({
                                                CheckedState: [
                                                  ...this.state.CheckedState,
                                                  tempobj,
                                                ],
                                              });
                                            }

                                            //this is used to exclude the already inpuut id

                                            if (
                                              this.state.MultiNotificationId.includes(
                                                item.NotificationId + ","
                                              )
                                            ) {
                                              var index =
                                                this.state.MultiNotificationId.indexOf(
                                                  item.NotificationId + ","
                                                );
                                              var part1 =
                                                this.state.MultiNotificationId.substring(
                                                  0,
                                                  index
                                                );
                                              var part2 =
                                                this.state.MultiNotificationId.substring(
                                                  index +
                                                    (item.NotificationId + ",")
                                                      .length,
                                                  this.state.MultiNotificationId
                                                    .length
                                                );

                                              this.setState({
                                                MultiNotificationId:
                                                  part1 + part2,
                                              });
                                            } else {
                                              this.setState({
                                                MultiNotificationId:
                                                  this.state.MultiNotificationId.concat(
                                                    item.NotificationId + ","
                                                  ),
                                              });
                                            }
                                          }}
                                          key={item.NotificationId}
                                          color="primary"
                                          value={this.state.MultiNotificationId}
                                          name="checked"
                                          //defaultChecked={false}
                                          checked={
                                            this.state.CheckedState.find(
                                              (obj) =>
                                                obj.NotificationID ==
                                                item.NotificationId
                                            ) == undefined
                                              ? false
                                              : this.state.CheckedState.find(
                                                  (obj) =>
                                                    obj.NotificationID ==
                                                    item.NotificationId
                                                )
                                          }
                                        />
                                      }
                                    />
                                  </TableCell>
                                  <TableCell
                                    style={CellStyle}
                                    //style={{ cursor: "pointer" }}
                                    onClick={() => {
                                      this.setState({
                                        showNotifcation: item.NotificationMsg,
                                      });
                                      this.setState({
                                        showNotifcationHeader:
                                          item.NotificationHeader,
                                      });
                                      this.setState({ showModel: true });
                                      this.ReadNotification(
                                        item.NotificationId
                                      );
                                    }}
                                  >
                                    {item.NotificationMsg}
                                  </TableCell>
                                </TableRow>
                              );
                            })
                          ) : (
                            <TableRow>
                              <TableCell colSpan="2">
                                You have zero Notification
                              </TableCell>
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                      
                    </TableContainer>
                    <TablePagination
                        component="div"
                        count={this.state.NotificationLength}
                        page={this.state.page}
                        onChangePage={this.handleChangePage}
                        rowsPerPage={this.state.rowsPerPage}
                        onChangeRowsPerPage={this.handleChangeRowsPerPage}
                      />
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
        <ShowNotificationPopUp
          Title={this.state.showNotifcationHeader}
          errorMsgs={this.state.showNotifcation}
          show={this.state.showModel}
          onHide={this.CloseModal}
        />
        <DeleteMultipleNotificationPopUp
          Title={"Delete Notification"}
          errorMsgs={this.state.showNotifcation}
          show={this.state.showMultiDeleteModel}
          ConfrimDelete={this.DeleteMultipleNotification}
          onHide={this.CloseModal}
        />
      </div>
    );
  }
}

export default Notification;
