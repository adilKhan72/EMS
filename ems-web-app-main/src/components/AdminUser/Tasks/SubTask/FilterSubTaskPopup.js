import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
class FilterSubTaskPopup extends Component {
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
            <option maintaskID={item.Id} value={item.MainTaskName}>
              {item.MainTaskName}
            </option>
          );
        })
      ) : (
        <option> No Project... </option>
      );
    return (
      <Modal
        show={this.props.FilterSubTaskShow}
        onHide={this.props.closeFilterSubTask}
        backdrop={"static"}
        keyboard={"false"}
        size="md"
        aria-labelledby="contained-modal-title-vcenter"
        centered
      >
        <div
          className="modal-content setting_search_popup "
          style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
        >
          <Modal.Header closeButton>
            <Modal.Title>
              <span className=" text-left">Filter SubTask</span>
              {/* <span
                style={{ cursor: "pointer" }}
                className="clear_btn"
                onClick={() => {
                  this.props.ClearSearch();
                }}
              >
                Clear Search
              </span> */}
            </Modal.Title>
          </Modal.Header>
          <Modal.Body className="material_style">
            <div className="col-md-12 p-0">
              <div className="form-group styled-select">
                <select
                  //className="form-control"
                  className="form-control"
                  name="ProjectName"
                  onChange={this.props.FilterSubTaskProjectHandleChange}
                  autoComplete="off"
                  value={this.props.ProjectNameDropdown}
                >
                  {/* {this.props.ProjectNameDropdown != null ? (
                    <option disabled selected hidden>
                      {this.props.ProjectNameDropdown}
                    </option>
                  ) : (
                    <option selected>---Select---</option>
                  )} */}
                  <option value="">All</option>
                  {ProjectNameList}
                </select>
                <label className="modal_label">Project</label>
              </div>
            </div>
            <div className="col-md-12 p-0">
              <div className="form-group styled-select">
                <select
                  //className="form-control"
                  className={"form-control "}
                  name="ProjectName"
                  onChange={this.props.FilterSubTaskMainTaskHandleChange}
                  autoComplete="off"
                  value={this.props.MainTaskNameDropdown}
                >
                  {/* {this.props.MainTaskNameDropdown != null ? (
                    <option disabled selected hidden>
                      {this.props.MainTaskNameDropdown}
                    </option>
                  ) : (
                    <option selected>---Select---</option>
                  )} */}
                  <option value="">All</option>
                  {MainTaskList}
                </select>
                <label className="modal_label">Main Task</label>
              </div>
            </div>
            <div className="form-group mb-0">
              <input
                className="form-control"
                type="text"
                name="TaskName"
                value={this.props.FilterSubTask.TaskName}
                onChange={this.props.FilterSubTaskHandleChange}
                required
              />

              <label>Sub Task</label>
              <div className="text-left ">
                <Button
                  className="btn-black"
                  onClick={this.props.FilterSubTaskFun}
                  style={{ marginBottom: "20px" }}
                >
                  Search
                </Button>
                <br />
              </div>
            </div>
          </Modal.Body>
        </div>
      </Modal>
    );
  }
}

export default FilterSubTaskPopup;
