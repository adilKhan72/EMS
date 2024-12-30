import React, { Component } from "react";
import { styling } from "../../PopUpMsgModal/stylepopup.css";
import { Modal, Button, Form } from "react-bootstrap";
class FilterModal extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    var TempprojectsList =
      this.props.ProjectList == null
        ? ""
        : this.props.ProjectList.map((item) => {
            return (
              <option value={item.ProjectID} name={item.ProjectName}>
                {item.ProjectName}
              </option>
            );
          });
    return (
      <>
        <Modal
          id="ems_exp_pdf"
          show={this.props.show}
          onHide={this.props.onHide}
          backdrop={"static"}
          keyboard={"false"}
          className={styling}
          size="lg"
          animation={false}
          centered
        >
          <div
            className="modal-content"
            style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
          >
            <Modal.Header closeButton>
              <Modal.Title>Project Filter</Modal.Title>
            </Modal.Header>
            <Modal.Body>
              <div className="row">
                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <label className="col-md-3 pl-0">Project</label>
                    <select
                      name="select"
                      onChange={this.props.changeprojectState}
                      id="select"
                      className="form-control pl-0"
                      value={this.props.ProjectId}
                    >
                      {this.props.ProjectId !== -1 ? (
                        <option value="" disabled selected hidden>
                          {this.props.ProjectId}
                        </option>
                      ) : (
                        <option value="">All Projects</option>
                      )}
                      {TempprojectsList}
                    </select>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <label>Status</label>
                    <select
                      className="form-control"
                      autoComplete="off"
                      name="Status"
                      value={this.props.FilterStatus}
                      onChange={this.props.ChangedStatus}
                    >
                      <option value={""}>All</option>
                      <option value={"true"}>Active</option>
                      <option value={"false"}>In-Active</option>
                    </select>
                  </div>
                </div>
              </div>
            </Modal.Body>
            <Modal.Footer>
              <Button className="btn-black" onClick={this.props.onSubmit}>
                Search
              </Button>
            </Modal.Footer>
          </div>
        </Modal>
      </>
    );
  }
}

export default FilterModal;
