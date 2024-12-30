import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";

class WBSExcelPopUpModal extends Component {
  constructor(props) {
    super(props);
  }

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
    return (
      <Modal
        show={this.props.show}
        onHide={this.props.onHide}
        backdrop={"static"}
        keyboard={"false"}
        centered
      >
        <div
          className="modal-content"
          style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
        >
          <Modal.Header closeButton>
            <Modal.Title>Export WBS in Excel</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            {" "}
            <div className="form-group mb-1 styled-select ">
              <label className="control-label">Project</label>
              <select
                className="form-control"
                id="projectDropdown"
                name="ProjectName"
                onChange={this.props.WBSExcelProjectDropdown}
                required
                value={this.props.WBSExcelProjectID}
              >
                <option value=""></option>
                {ProjectNameList}
              </select>
            </div>
          </Modal.Body>
          <Modal.Footer>
            <Button className="btn-black" onClick={this.props.onConfrim}>
              Export
            </Button>
          </Modal.Footer>
        </div>
      </Modal>
    );
  }
}

export default WBSExcelPopUpModal;
