import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
class AddSubTaskPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    var ProjectNameList =
      this.props.lstProjectName !== null ? (
        this.props.lstProjectName.map((item) => {
          return item.Active == false ? null : (
            <option ddattrprojectid={item.ID} value={item.Name}>
              {item.Name}
            </option>
          );
        })
      ) : (
        <option> No Project... </option>
      );

    var MainTaskList =
      this.props.lstMainTask !== null ? (
        this.props.lstMainTask.map((item) => {
          return item.Active == false ? null : (
            <option value={item.Id} TaskName={item.MainTaskName}>
              {item.MainTaskName}
            </option>
          );
        })
      ) : (
        <option> No Project... </option>
      );
    return (
      <Modal
        show={this.props.AddSubTaskShow}
        onHide={this.props.closeAddSubTask}
        backdrop={"static"}
        keyboard={"false"}
        size="md"
        aria-labelledby="contained-modal-title-vcenter"
        centered
      >
        <div className="modal-content">
          <Modal.Header
            closeButton
            style={{ border: "none", padding: "0.2rem" }}
          >
            <h5 className="p-3 pl-4 pb-0" style={{ fontWeight: "600" }}>
              {this.props.AddSubTaskTitle}
            </h5>
          </Modal.Header>
          <Modal.Body className="ems_add_subtask">
            <div className="modal-body material_style">
              <div className="row">
                <div className="col-md-12">
                  <div className="form-group styled-select">
                    <select
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.ProjectName
                      }
                      name="ProjectName"
                      onChange={this.props.AddSubTaskHandleChange}
                      required
                      autoComplete="off"
                      onFocus={() => {
                        this.props.HandelErrorRemove("ProjectName");
                      }}
                    >
                      {this.props.AddSubTaskHandle.ProjectName != null ? (
                        <option disabled selected hidden>
                          {this.props.AddSubTaskHandle.ProjectName}
                        </option>
                      ) : (
                        <option selected>---Select---</option>
                      )}
                      {ProjectNameList}
                    </select>
                    <label className="modal_label">Project</label>
                  </div>
                </div>

                <div className="col-md-12">
                  <div className="form-group styled-select">
                    <select
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.MainTaskName
                      }
                      name="MainTaskID"
                      onChange={this.props.AddSubTaskHandleChange}
                      required
                      autoComplete="off"
                      onFocus={() => {
                        this.props.HandelErrorRemove("MainTaskID");
                      }}
                    >
                      {this.props.AddSubTaskHandle.MainTaskName != null ? (
                        <option disabled selected hidden>
                          {this.props.AddSubTaskHandle.MainTaskName}
                        </option>
                      ) : (
                        <option selected>---Select---</option>
                      )}
                      {MainTaskList}
                    </select>
                    <label className="modal_label">Main Task</label>
                  </div>
                </div>
                <div className="col-md-12">
                  <div className="form-group ">
                    <input
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.SubTaskName
                      }
                      type="text"
                      name="SubTaskName"
                      value={this.props.AddSubTaskHandle.SubTaskName}
                      onChange={this.props.AddSubTaskHandleChange}
                      required
                      autoComplete="off"
                      onFocus={() => {
                        this.props.HandelErrorRemove("SubTaskName");
                      }}
                    />
                    <label className="modal_label">Task/PBI</label>
                  </div>
                </div>
                <div className="col-md-12">
                  <div className="form-group ems_sub_task_budget">
                    <input
                      className={"form-control"}
                      type="text"
                      name="EstimatedDuration"
                      value={this.props.AddSubTaskHandle.EstimatedDuration}
                      onInput={this.props.AddSubTaskHandleChange.bind(this)}
                      required
                      autoComplete="off"
                      maxLength={4}
                      pattern="[0-9.]*"
                    />
                    <label className="modal_label">Budget (Hrs)</label>
                  </div>
                </div>
                <div className="col-md-12">
                  <div className="form-group ems_comment_sec">
                    <label className="modal_label">Comments</label>
                    <textarea
                      className="ui-widget form-control txtArea text_boxshadow"
                      name="Comments"
                      value={this.props.AddSubTaskHandle.Comments}
                      onChange={this.props.AddSubTaskHandleChange}
                      autoComplete="off"
                      rows="3"
                      cols="50"
                      style={{ resize: "none" }}
                      // onFocus={() => {
                      //   this.HandelErrorRemove("CommentFiled");
                      // }}
                      required
                    ></textarea>

                    {/* <input
                      //className="form-control"
                      className={"form-control"}
                      type="text"
                      name="Comments"
                      value={this.props.AddSubTaskHandle.Comments}
                      onChange={this.props.AddSubTaskHandleChange}
                      required
                      autoComplete="off"
                    />
                    <label className="modal_label">Comments</label> */}
                  </div>
                </div>
                <div className="col-md-12">
                  <div
                    className="form-group"
                    style={{
                      display: "flex",
                      alignItems: "center",
                      padding: "0px",
                    }}
                  >
                    <button
                      className="btn-black mr-1"
                      onClick={this.props.AddSubTaskFun}
                    >
                      {this.props.AddSubtaskButton}
                    </button>
                    {this.props.AddSubTaskHandle.Id != 0 ? (
                      <button
                        className=" btn-black mr-1"
                        onClick={this.props.DeleteSubTaskFun}
                      >
                        Delete SubTask
                      </button>
                    ) : null}
                  </div>
                </div>
              </div>
            </div>
          </Modal.Body>
        </div>
      </Modal>
    );
  }
}

export default AddSubTaskPopup;
