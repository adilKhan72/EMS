import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
/* import "date-fns";
import DateFnsUtils from "@date-io/date-fns";
import {
  MuiPickersUtilsProvider,
  KeyboardTimePicker,
  KeyboardDatePicker,
} from "@material-ui/pickers"; */

class FilterMainTaskPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <Modal
        show={this.props.FilterMainTaskPopup}
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
              <span className=" text-left">Filter MainTask</span>
              {/* <span
                className="clear_btn"
                onClick={() => {
                  this.props.ClearSearch();
                }}
                style={{ cursor: "pointer" }}
              >
                Clear Search
              </span> */}
            </Modal.Title>
          </Modal.Header>
          <Modal.Body className="material_style">
            <div className="form-group mb-0">
              <input
                className="form-control"
                type="text"
                name="TaskName"
                value={this.props.FilterMainTask.MainTaskName}
                onChange={this.props.FilterMainTaskHandleChange}
                required
              />
              <label>Search Task</label>
              <div className="text-left ">
                <Button
                  className="btn-black"
                  onClick={this.props.FilterMainTaskFun}
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

export default FilterMainTaskPopup;
