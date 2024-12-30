import React, { Component } from "react";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import CreateNotificationPopup from "./CreateNotificationPopup";
import "./NotificationCreation.css";

class NotificationCreation extends Component {
  constructor(props) {
    super(props);
    this.state = {
      CreateNotificationPopupShow: false,
    };
  }

  render() {
    return (
      <>
        <div className="content mt-3">
          <div className="row">
            <div className="col-4">
              <h2 class="m_font_heading">Notification Listing</h2>
            </div>
            <div className="col-8">
              <button
                className="btn-black mr-1"
                type="button"
                style={{ float: "right" }}
                onClick={() => {
                  this.setState({ CreateNotificationShow: true });
                }}
                data-toggle="modal"
                data-target="#CreateNotification"
              >
                Notification Creation
                <i className="menu-icon fa fa-plus"></i>
              </button>
            </div>
          </div>
          <div className="row" style={{ marginTop: "30px" }}>
            <div className="col-12">
              <div className="card p-2">
                <div className="card-body">
                  <TableContainer
                    className="table-responsive"
                    component={"div"}
                  >
                    <Table className="table notification_tbl">
                      <TableHead>
                        <TableRow>
                          <TableCell>Notification ID</TableCell>
                          <TableCell>Notification Text</TableCell>
                          <TableCell>Start Time</TableCell>
                          <TableCell>End Time</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        <TableRow>
                          <TableCell>Notification 23422</TableCell>
                          <TableCell>
                            Lorem ipsum dolor sit amet, consectetur adipiscing
                            elit. Duis facilisis sollicitudin cursus. Donec at
                            magna euismod orci fringilla accumsan sit amet eget
                            nunc. In eget purus dui. Maecenas volutpat tellus
                            non tempor congue.{" "}
                          </TableCell>
                          <TableCell>9:20AM </TableCell>
                          <TableCell>6:00PM</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell>Notification 23422</TableCell>
                          <TableCell>
                            Lorem ipsum dolor sit amet, consectetur adipiscing
                            elit. Duis facilisis sollicitudin cursus. Donec at
                            magna euismod orci fringilla accumsan sit amet eget
                            nunc. In eget purus dui. Maecenas volutpat tellus
                            non tempor congue.{" "}
                          </TableCell>
                          <TableCell>9:20AM </TableCell>
                          <TableCell>6:00PM</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell>Notification 23422</TableCell>
                          <TableCell>
                            Lorem ipsum dolor sit amet, consectetur adipiscing
                            elit. Duis facilisis sollicitudin cursus. Donec at
                            magna euismod orci fringilla accumsan sit amet eget
                            nunc. In eget purus dui. Maecenas volutpat tellus
                            non tempor congue.{" "}
                          </TableCell>
                          <TableCell>9:20AM </TableCell>
                          <TableCell>6:00PM</TableCell>
                        </TableRow>
                        <TableRow>
                          <TableCell>Notification 23422</TableCell>
                          <TableCell>
                            Lorem ipsum dolor sit amet, consectetur adipiscing
                            elit. Duis facilisis sollicitudin cursus. Donec at
                            magna euismod orci fringilla accumsan sit amet eget
                            nunc. In eget purus dui. Maecenas volutpat tellus
                            non tempor congue.{" "}
                          </TableCell>
                          <TableCell>9:20AM </TableCell>
                          <TableCell>6:00PM</TableCell>
                        </TableRow>
                      </TableBody>
                    </Table>
                  </TableContainer>
                </div>
              </div>
            </div>
          </div>
          <CreateNotificationPopup
            CreateNotificationShow={this.state.CreateNotificationShow}
            closeCreateNotification={() => {
              this.setState({ CreateNotificationShow: false });
            }}
          />
        </div>
      </>
    );
  }
}
export default NotificationCreation;
