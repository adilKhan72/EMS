import React, { Component } from "react";
import { Modal, Button } from "react-bootstrap";
import ReportFilterDesign from "./ReportFilterDesign";
import ReportGrid from "./ReportGrid";

class ReportFilter extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    console.log("Report Filter Render Method");
    return (
      <div>
        <Modal
        style={{overflow: "hidden"}}
          show={this.props.show}
          onHide={this.props.onHide}
          backdrop={"static"}
          keyboard={"false"}
          size="lg"
          aria-labelledby="contained-modal-title-vcenter"
          centered
        >
          <div
            className="modal-content"
            style={{ boxShadow: "1px 5px 17px 5px #acabb1" }}
          >
            <Modal.Header closeButton>
              <Modal.Title>{this.props.Title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>

              <ReportFilterDesign
                projectDropDown={this.props.project}
                maintaskDropDown={this.props.maintasks}
                projectchangeEvent={this.props.projectchangeEvent}
                taskowernamesDropDown={this.props.taskowernames}
                StartReportGridDateFormat={
                  this.props.StartReportGridDateFormatFunc
                }
                EndReportGridDateFormat={this.props.EndReportGridDateFormatFunc}
                ReportfilterProjectDD={this.props.ProjectDropDownValue}
                ReportfilterHourDayDD={this.props.HoursDaysDropDownValue}
                ReportfilterHourDayDDshow={
                  this.props.HoursDaysDropDownValueShow
                }
                
                projectDDSelectedValue={this.props.projectDDSelectedValue}
                projectDDSelectedValueShows={
                  this.props.projectDDSelectedValueShow
                }
                showStartDates={this.props.showStartDate}
                showEndDates={this.props.showEndDate}
                ReportHandleChange={this.props.AdminHandleChange}z
                CheckboxHandleChange={this.props.AdminHandleChangecheckbox}
                onchangehandle={this.props.onchangehandle}
                getDropdownButtonLabel={this.props.getDropdownButtonLabel}
                lstTaskOwnerName_List={this.props.lstTaskOwnerName_List}
                setSelectedOptions={this.props.setSelectedOptions}
                Selectedtaskowner={this.props.Selectedtaskowner}

                lstSubtask={this.props.lstSubtask}
                ShowSelectedDataAdmin={this.props.ShowSelectedDataAdmin}
                ClientList={this.props.ClientList}
                ClientIdChange={(e) => {
                  this.props.ClientIdChange(e.target.value);
                }}
                ClientIDSelected={this.props.ClientIDSelected}
                SelectExportTypeFun={this.props.SelectExportTypeFun}
                lstDepartmentNames={this.props.lstDepartmentNames}
              />
            </Modal.Body>
            <Modal.Footer>
              <Button className="btn-black" onClick={this.props.RefreshCall}>
                Search
              </Button>
              <Button className="btn-black" onClick={this.props.onHide}>
                Cancel
              </Button>
            </Modal.Footer>
          </div>
        </Modal>
      </div>
    );
  }
}

export default ReportFilter;
