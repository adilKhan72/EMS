import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "./DateCalander.css";
import Checkbox from "@material-ui/core/Checkbox";
import Tooltip from "@material-ui/core/Tooltip";
import Cookies from "js-cookie";
import axios from "axios";
import ConfirmPopModal from "./ConfirmPopUpModal";
const errorstyle = {
  borderStyle: "solid",
  borderWidth: "2px",
  borderColor: "Red",
};
class UpdateAddRecord extends Component {
  constructor(props) {
    super(props);
    this.state = {
      PopUpBit: false,
      Title: "",
      errorMsg: "",
      ConfirmSave: true,
      ConfrimMsgPopUp: false,
      DropdownData: null,
      UserIDassigned: null,
      UserNameAssigned: null,
      ProjectIDs: null,
      ProjectNames: "",
      triggertask: false,
      ActualDuration: "",
      Comment: "",
      APIProjectDate: "",

      errorStyle: {
        ProjectDD: "",
        ResourceNameDD: "",
        ProjectDescription: "",
        ActualDurationFiled: "",
        ProjectDatefield: "",
        PhaseFiled: "",
        DescriptionFiled: "",
        CommentFiled: "",
      },
    };
  }
  componentDidMount() {}
  CheckConfirm = () => {
    this.props.UpdateProjectData.TaskOwnerID = this.state.UserIDassigned;
    this.props.UpdateProjectData.TaskOwnerName = this.state.UserNameAssigned;
    this.setState(
      {
        ConfirmSave: true,
        ConfrimMsgPopUp: false,
      },
      () => {
        if (this.state.triggertask === true) {
          this.props.RecordChange();
        }

        this.props.SaveRecordUpdate(true);
      }
    );
  };
  HandelErrorRemove = (name) => {
    if (name == "ProjectDatefield") {
      if (this.state.ProjectDatefield == "") {
        this.setState((prevState) => ({
          errorStyle: {
            ...prevState.errorStyle,
            ProjectDatefield: "",
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
  };
  CloseModal = () => {
    // const Obj = {
    //   ProjectName: this.props.UpdateProjectData.ProjectName,
    //   ProjectID: this.props.UpdateProjectData.ProjectID,
    // };
    this.setState({ ConfrimMsgPopUp: false });
    this.props.UpdateProjectData.ProjectName = "";
    this.props.UpdateProjectData.ProjectID = null;
    // this.props.EmptyProjectDropdown();
    if (this.state.triggertask === true) {
      this.props.RecordChange();
    }
    //this.props.SaveRecordUpdate(true);
    this.props.SaveRecordUpdate(false);
  };

  CheckAssignedUserWhenProjectChanged = (e) => {
    try {
      var projectID =
        e.target[e.target.selectedIndex].getAttribute("projectid");
      this.setState({
        DropdownData: e.target,
        ProjectId: projectID,
        triggertask: true,
      });
      var UserName = this.props.UpdateProjectData.TaskOwnerName;
      var userid = this.props.UpdateProjectData.TaskOwnerID;

      const CheckUsersData = {
        UserID: userid,
        ProjectID: projectID,
      };
      this.props.UpdateProjectData.ProjectName = e.target.value;
      this.props.UpdateProjectData.ProjectID = projectID;
      if (UserName != Cookies.get("UserName")) {
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
            this.props.UpdateProjectData.TaskOwnerID = userid;
            this.props.UpdateProjectData.TaskOwnerName = UserName;
            this.props.RecordChange();
            this.props.SaveRecordUpdate(true);
            // this.props.UpdateHandleChange(this.state.DropdownData);
            // this.setState({ ResourceName: UserName, TaskOwnerID: userid });
          }
          if (res.data.StatusCode === 404) {
            this.setState({
              Title: "Add Record",
              ConfirmSave: false,
              UserIDassigned: userid,
              UserNameAssigned: UserName,
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
        this.props.UpdateProjectData.TaskOwnerID = userid;
        this.props.UpdateProjectData.TaskOwnerName = UserName;
        this.props.RecordChange();
      }
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  CheckAssignedUser = (e) => {
    try {
      this.setState({ DropdownData: e.target, triggertask: false });
      var UserName = e.target[e.target.selectedIndex].label;
      var userid = e.target.value;
      const CheckUsersData = {
        UserID: userid,
        ProjectID: this.props.UpdateProjectData.ProjectID,
      };
      if (UserName != Cookies.get("UserName")) {
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
            this.props.UpdateProjectData.TaskOwnerID = userid;
            this.props.UpdateProjectData.TaskOwnerName = UserName;
            // this.props.RecordChange();
            this.props.SaveRecordUpdate(true);
            // this.props.UpdateHandleChange(this.state.DropdownData);
            // this.setState({ ResourceName: UserName, TaskOwnerID: userid });
          }
          if (res.data.StatusCode === 404) {
            this.setState({
              Title: "Add Record",
              ConfirmSave: false,
              UserIDassigned: userid,
              UserNameAssigned: UserName,
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
        this.props.UpdateProjectData.TaskOwnerID = userid;
        this.props.UpdateProjectData.TaskOwnerName = UserName;
        this.props.RecordChange();
      }
    } catch (ex) {
      window.location.href = "/Error";
    }
  };
  render() {
    var ProjectNameList =
      this.props.lstProjectNames != null
        ? this.props.lstProjectNames.map((item) => {
            return (
              <option value={item.Name} projectId={item.ID}>
                {item.Name}
              </option>
            );
          })
        : null;

    var TaskOwnerNameList =
      this.props.lstTaskOwnerName != null
        ? this.props.lstTaskOwnerName.map((item) => {
            return (
              <option value={item.UserProfileTableID} taskOwnerName={item.Name}>
                {item.Name}
              </option>
            );
          })
        : null;

    /*  var MainTaskNameList =
      this.props.lstMainTaskName == null ? (
        <option value="" disabled selected hidden>
          {this.props.UpdateProjectData.Maintask}
        </option>
      ) : (
        this.props.lstMainTaskName.map((item) => {
            var ProjectIdFromTask = item.ProjectIDs;
            if (
              this.props.UpdateProjectData.ProjectID !== null &&
              this.props.UpdateProjectData.Maintask !== ""
            ) {
              var splitArr = ProjectIdFromTask.split(",");
              for (var i = 0; i < splitArr.length; i++) {
                if (this.props.UpdateProjectData.ProjectID == splitArr[i]) {
                  return (
                    <option value={item.Id} ddAttrMainTaskID={item.MainTaskName}>
                      {item.MainTaskName}
                    </option>
                  );
                }
              }
            } else {
              return (
                <option value={item.Id} ddAttrMainTaskID={item.MainTaskName}>
                  {item.MainTaskName}
                </option>
              );
            }

          return (
            <option value={item.Id} ddAttrMainTaskID={item.MainTaskName}>
              {item.MainTaskName}
            </option>
          );
        })
      );
 */

    var MainTaskNameList =
      this.props.lstMainTaskName != null
        ? this.props.lstMainTaskName.map((item) => {
            return (
              <option value={item.Id} ddAttrMainTaskID={item.MainTaskName}>
                {item.MainTaskName}
              </option>
            );
          })
        : null;

    var SubTaskNameList =
      this.props.lstSubTaskName != null
        ? this.props.lstSubTaskName.map((item) => {
            return <option value={item.SubtaskId}>{item.TaskName}</option>;
          })
        : null;

    return (
      <Modal
        show={this.props.show}
        onHide={this.props.onHide}
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
            <Modal.Title>Update Record</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            <div className="col-md-12 material_style pt-4 pl-0">
              <form className="form-horizontal row">
                <div className="col-md-6">
                  <div
                    /* className="form-group styled-select mb-1" */
                    className={
                      this.props.UpdateProjectData.ProjectName
                        ? "form-group mb-1 styled-select "
                        : "form-group mb-1 styled-select input_error"
                    }
                  >
                    <select
                      className="form-control"
                      id="projectDropdown"
                      name="ProjectName"
                      onChange={this.CheckAssignedUserWhenProjectChanged}
                      required
                      value={this.props.UpdateProjectData.ProjectName}
                    >
                      <option value="" disabled selected hidden>
                        {this.props.UpdateProjectData.ProjectName}
                      </option>
                      {ProjectNameList}
                    </select>
                    <label className="control-label">Project</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group styled-select mb-1">
                    <select
                      className="form-control"
                      id="resourceDropdown"
                      // id="resourceDropdown"
                      name="TaskOwnerID"
                      //onChange={this.props.UpdateHandleChange}
                      onChange={this.CheckAssignedUser}
                      // required
                      value={this.props.UpdateProjectData?.TaskOwnerID}
                    >
                      <option value="" disabled selected hidden>
                        {/* {this.props.UpdateProjectData?.TaskOwnerName
                          ? this.props.UpdateProjectData.TaskOwnerName
                          : "---Select---"} */}
                      </option>

                      {TaskOwnerNameList}
                    </select>
                    <label className="control-label" for="textinput">
                      Resource
                    </label>
                  </div>
                </div>
                <div className="col-md-12">
                  <div
                    className={
                      this.props.UpdateHandleChange == ""
                        ? "form-group  textarea_mbl "
                        : "form-group  textarea_mbl txtArea_value_added"
                    }
                  >
                    <textarea
                      className="ui-widget form-control txtArea text_boxshadow"
                      id="txtSubTaskArea"
                      rows="3"
                      cols="50"
                      name="ProjectDescription"
                      onChange={this.props.UpdateHandleChange}
                      required
                      readOnly
                    >
                      {this.props.UpdateProjectData.ProjectDescription}
                    </textarea>
                    <label className="modal_label" style={{ width: "165px" }}>
                      Project Description
                    </label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div
                    className={
                      this.props.UpdateProjectData.ProjectDate === false
                        ? "form-group mb-1 date_value_added " +
                          this.props.errorStyle.ProjectDatefield
                        : "form-group " + this.props.errorStyle.ProjectDatefield
                    }
                  >
                    <div className="customDatePickerWidth ">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        shouldCloseOnSelect={true}
                        maxDate={new Date()}
                        value={this.props.UpdateProjectData.ProjectDate}
                        selected={this.props.UpdateProjectData.ProjectDate}
                        //onSelect={this.state.showSelectedDate}
                        className="form-control input-md "
                        onChange={this.props.UpdateStartHandleDate}
                        required
                      />
                      <label className="datepicker_label">Date</label>
                    </div>
                  </div>
                </div>

                <div className="col-md-5">
                  <div
                    className={
                      this.props.UpdateActualDurationHandleChange
                        ? "form-group addrecordinput_label " +
                          this.props.errorStyle.ActualDurationFiled
                        : "form-group addrecordinput_label " +
                          this.props.errorStyle.ActualDurationFiled
                    }
                  >
                    <input
                      className="form-control"
                      value={this.props.UpdateProjectData.ActualDuration}
                      name="ActualDuration"
                      // placeholder="Duration (1-8)"
                      type="text"
                      pattern="[0-9.]*"
                      onChange={this.props.UpdateActualDurationHandleChange}
                      onFocus={() => {
                        this.HandelErrorRemove("ActualDurationFiled");
                      }}
                      //defaultValue={this.props.UpdateProjectData.ActualDuration}
                      required
                    />
                    <label className="control-label">Actual Durations</label>
                  </div>
                </div>

                <div className="col-md-1">
                  <div className="ems_counter">
                    <Tooltip
                      title="Check for Billable"
                      className="popup_billable"
                      arrow
                    >
                      <Checkbox
                        inputProps={{ "aria-label": "uncontrolled" }}
                        onClick={() => {
                          this.props.UpdateRecordCheckboxHandleFun();
                        }}
                        checked={this.props.UpdateProjectData.UpdateCheckBox}
                      />
                    </Tooltip>
                    <span className="houres_box">
                      {this.props.UpdateProjectData.BillableHours}
                    </span>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group mb-1 styled-select">
                    <select
                      className="ui-widget form-control chosen"
                      id="MaintasksDropdown"
                      name="MainTaskID"
                      onChange={this.props.UpdateHandleChange}
                      value={this.props.UpdateProjectData.MainTaskID}
                    >
                      {/* <option value="" disabled selected hidden>
                        {this.props.UpdateProjectData.Maintask}
                      </option> */}
                      {MainTaskNameList}
                    </select>
                    <label className="control-label">Main Task</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group mb-1 styled-select">
                    <select
                      className=" ui-widget form-control chosen"
                      id="tasksDropdown"
                      name="SubtaskID"
                      onChange={this.props.UpdateHandleChange}
                    >
                      {this.props.UpdateProjectData.Subtask == "" ? (
                        SubTaskNameList
                      ) : (
                        <>
                          <option value="" disabled selected hidden>
                            {this.props.UpdateProjectData.Subtask}
                          </option>
                          {SubTaskNameList}
                        </>
                      )}
                    </select>
                    <label className="control-label">Task/PBI</label>
                  </div>
                </div>
                <div className="col-md-12">
                  <div
                    className={
                      this.props.UpdateHandleChange
                        ? "form-group mb-1  textarea_mbl " +
                          this.props.errorStyle.CommentFiled
                        : "form-group  textarea_mbl " +
                          this.props.errorStyle.CommentFiled
                    }
                  >
                    <textarea
                      className="ui-widget form-control txtArea text_boxshadow"
                      id="txtSubTaskArea"
                      rows="3"
                      cols="50"
                      name="Comment"
                      onChange={this.props.UpdateHandleChange}
                      onFocus={() => {
                        this.HandelErrorRemove("CommentFiled");
                      }}
                      required
                    >
                      {this.props.UpdateProjectData.Comment}
                    </textarea>
                    <label className="modal_label">Description</label>
                  </div>
                </div>
              </form>
            </div>
          </Modal.Body>
          <Modal.Footer>
            <Button
              className="btn-black"
              onClick={this.props.UpdateRecordsApproved}
            >
              Approved
            </Button>
            <Button className="btn-black" onClick={this.props.UpdateRecord}>
              Save
            </Button>
            <Button className="btn-black" onClick={this.props.DeleteRecord}>
              Delete
            </Button>
            {/* <Button className="btn-black" onClick={this.props.onHide}>
              Cancel
            </Button> */}
          </Modal.Footer>
          <ConfirmPopModal
            Title={this.state.Title}
            ShowMsgs={this.state.errorMsg}
            show={this.state.ConfrimMsgPopUp}
            onConfrim={this.CheckConfirm}
            onHide={this.CloseModal}
          />
        </div>
      </Modal>
    );
  }
}
export default UpdateAddRecord;
