import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import Radio, { RadioProps } from "@material-ui/core/Radio";
import Checkbox from "@material-ui/core/Checkbox";
class Exportmodal extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <Modal
        show={this.props.ExportShow}
        onHide={this.props.ExportonHide}
        backdrop={"static"}
        keyboard={"false"}
        centered
      >
        <div
          className="modal-content"
          style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
        >
          <Modal.Header closeButton>
            <Modal.Title>Export Report</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            <>
              <div key={`inline-radio`} className="mb-3">
                <Form.Check
                  inline
                  label="Actual"
                  name="Bill_Actual"
                  type={"radio"}
                  id={`Actual`}
                  value={`Actual`}
                  onClick={this.props.Bill_ActualFun}
                  checked={this.props.Bill_Actual == "Actual" ? true : false}
                />
                <Form.Check
                  inline
                  label="Billable"
                  name="Bill_Actual"
                  type={"radio"}
                  id={`Billable`}
                  value={`Billable`}
                  onClick={this.props.Bill_ActualFun}
                  checked={this.props.Bill_Actual == "Billable" ? true : false}
                />
              </div>
              <Checkbox
                onClick={this.props.CheckBoxFun}
                // onClick={() => {
                //   if (this.state.ShowZeroHours == true) {
                //     this.setState({ ShowZeroHours: false });
                //   } else {
                //     this.setState({ ShowZeroHours: true });
                //   }
                // }}
                disabled={this.props.Bill_Actual == "Actual" ? true : false}
                checked={this.props.ShowZeroHours}
              />
              Show 0 Hours
              <hr />
              <label>
                <strong> Reports Type</strong>
              </label>
              <div key={`inline-radio`} className="mb-3">
                <Form.Check
                  inline
                  label="By Client"
                  name="ExportType"
                  type={"radio"}
                  id={`By_Client`}
                  value={`By_Client`}
                  onClick={this.props.Export_TypeFun}
                  checked={this.props.Export_Type == "By_Client" ? true : false}
                />
                <Form.Check
                  inline
                  label="By Project"
                  name="ExportType"
                  type={"radio"}
                  id={`By_Project`}
                  value={`By_Project`}
                  onClick={this.props.Export_TypeFun}
                  checked={
                    this.props.Export_Type == "By_Project" ? true : false
                  }
                />
                <Form.Check
                  inline
                  label="By TaskOwner"
                  name="ExportType"
                  type={"radio"}
                  id={`By_TaskOwner`}
                  value={`By_TaskOwner`}
                  onClick={this.props.Export_TypeFun}
                  checked={
                    this.props.Export_Type == "By_TaskOwner" ? true : false
                  }
                />
              </div>
              <div key={`inline-radio`} className="mb-3">
                <Form.Check
                  inline
                  label="WBS"
                  name="ExportType"
                  type={"radio"}
                  id={`By_WBS`}
                  value={`By_WBS`}
                  onClick={this.props.Export_TypeFun}
                  checked={this.props.Export_Type == "By_WBS" ? true : false}
                  disabled={this.props.Bill_Actual == "Billable" ? true : false}
                />
                <Form.Check
                  inline
                  label="MainTask BreakDown"
                  name="ExportType"
                  type={"radio"}
                  id={`By_MainTask_BreakDown`}
                  value={`By_MainTask_BreakDown`}
                  onClick={this.props.Export_TypeFun}
                  checked={
                    this.props.Export_Type == "By_MainTask_BreakDown"
                      ? true
                      : false
                  }
                  disabled={this.props.Bill_Actual == "Actual" ? true : false}
                />
              </div>
              <hr />
              <div key={`inline-radio`} className="mb-3">
                <Form.Check
                  inline
                  label="Excel"
                  name="Media"
                  type={"radio"}
                  id={`Excel`}
                  value={`Excel`}
                  onClick={this.props.Export_MediaFun}
                  checked={this.props.Export_Media == "Excel" ? true : false}
                />
                <Form.Check
                  inline
                  label="PDF"
                  name="Media"
                  type={"radio"}
                  id={`PDF`}
                  value={`PDF`}
                  onClick={this.props.Export_MediaFun}
                  checked={this.props.Export_Media == "PDF" ? true : false}
                  disabled={this.props.Bill_Actual == "Actual" ? true : false}
                />
              </div>
            </>
          </Modal.Body>
          <Modal.Footer>
            <Button className="btn-black" onClick={this.props.onExport}>
              Export
            </Button>
            <Button className="btn-black" onClick={this.props.ExportonHide}>
              Cancel
            </Button>
          </Modal.Footer>
        </div>
      </Modal>
    );
  }
}

export default Exportmodal;
