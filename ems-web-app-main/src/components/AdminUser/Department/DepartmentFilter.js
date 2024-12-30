import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";

class DepartmentFilter extends Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    var TempdepartmentList =
      this.props.DepartList == null
        ? ""
        : this.props.DepartList.map((item) => {
            return (
              <option value={item.ID} name={item.DepartmentName}>
                {item.DepartmentName}
              </option>
            );
          });
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
        <div className="modal-content">
          <Modal.Header
            closeButton
            style={{ border: "none", padding: "0.2rem" }}
          >
            <h5 className="p-3 pl-4 pb-0" style={{ fontWeight: "600" }}>
              {this.props.Title}
            </h5>
          </Modal.Header>
          <Modal.Body className="ems_update_client">
            <div className="row">
              <div className="col-md-6 mt-3">
                <div className="form-group styled-select">
                  <label className="col-md-3 pl-0">Departments</label>
                  <select
                    name="select"
                    onChange={this.props.changedepartmentState}
                    id="select"
                    className="form-control pl-0"
                    value={this.props.SelectedDepartment}
                  >
                    {this.props.SelectedDepartment !== -1 ? (
                      <option value="" disabled selected hidden>
                        {this.props.SelectedDepartment}
                      </option>
                    ) : (
                      <option value="">All Departments</option>
                    )}
                    {TempdepartmentList}
                  </select>
                </div>
              </div>
              <div className="col-md-6 mt-3">
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
    );
  }
}

export default DepartmentFilter;
