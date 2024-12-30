import React, { Component } from "react";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "../AddRecord/DateCalander.css";
import moment from "moment";
import Cookies from "js-cookie";
import ReactMultiSelectCheckboxes from 'react-multiselect-checkboxes';

class ReportFilterDesign extends Component {
  
  constructor(props) {
    super(props);
    this.state = {
      projectid: -1,
      maintaskid: -1,
      lstSubtask: null,
      selectedOptions: [],
    
    };
    this.onFocus = this.onFocus.bind(this);
    this.onBlur = this.onBlur.bind(this);
  }
  TaskOwnerNameList =
  this.props.taskowernamesDropDown == null
    ? ""
    : this.props.taskowernamesDropDown.map((item) => {
        return { value: item.UserProfileTableID, label: item.Name }            
      });
// componentDidMount(){
//   debugger
// var abc= this.props.setSelectedOptions
//   alert(this.props.Selectedtaskowner);
// }
  onFocus(e) {
    e.currentTarget.type = "date";
    e.currentTarget.focus();
  }
  onBlur(e) {
    e.currentTarget.type = "text";
    e.currentTarget.blur();
  }
  render() {
    var TempprojectsList =
      this.props.projectDropDown == null
        ? ""
        : this.props.projectDropDown.map((item) => {
            return (
              <option value={item.ID} name={item.Name}>
                {item.Name}
              </option>
            );
          });
    var ClientListDropdown =
      this.props.ClientList == null
        ? ""
        : this.props.ClientList.map((item) => {
            return (
              <option value={item.ID} name={item.ClientName}>
                {item.ClientName}
              </option>
            );
          });
    var TempmainTaskList =
      this.props.maintaskDropDown == null
        ? ""
        : this.props.maintaskDropDown.map((item) => {
            if (item.Active == true) {
              var ProjectIdFromTask = item.ProjectIDs;
              if (
                this.props.projectDDSelectedValue !== null &&
                this.props.projectDDSelectedValue !== ""
              ) {
                var splitArr = ProjectIdFromTask.split(",");
                for (var i = 0; i < splitArr.length; i++) {
                  if (this.props.projectDDSelectedValue === splitArr[i]) {
                    return (
                      <option value={item.Id} ddAttrMainTaskID={item.Id}>
                        {item.MainTaskName}
                      </option>
                    );
                  }
                }
              } else {
                return (
                  <option ddAttrMainTaskID={item.Id} value={item.Id}>
                    {item.MainTaskName}
                  </option>
                );
              }
            }
          });

    // /*  var TempmainTaskList =
    //   this.props.maintaskDropDown == null
    //     ? ""
    //     : this.props.maintaskDropDown.map((item) => {
    //         return <option value={item.Id}>{item.MainTaskName}</option>;
    //       });

    var SubTaskList =
      this.props.lstSubtask == null
        ? ""
        : this.props.lstSubtask.map((item) => {
            return <option value={item.SubtaskId}>{item.TaskName}</option>;
          });
    //       
    // var TaskOwnerNameList =
    //   this.props.taskowernamesDropDown == null
    //     ? ""
    //     : this.props.taskowernamesDropDown.map((item) => {
    //         return <option value={item.UserProfileTableID}>{item.Name}</option>;
    //       });
    

    var lstDepartmentNames =
      this.props.lstDepartmentNames == null
        ? ""
        : this.props.lstDepartmentNames.map((item) => {
            return <option value={item.ID}>{item.DepartmentName}</option>;
          });
    return (
      <div className="modal-body material_style pl-0" 
      id="ems_reports_filter_ui"
      >
        <div className="row">
          {Cookies.get("Role") == "Admin" ||
          Cookies.get("Role") == "SuperAdmin" ? (
            <>
              <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    name="select"
                    onChange={this.props.ClientIdChange}
                    id="select"
                    value={this.props.ClientIDSelected}
                    className="form-control"
                  >
                    <option value="">All</option>
                    {ClientListDropdown}
                  </select>
                  <label className="modal_label">Client</label>
                </div>
              </div>
              <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    name="select"
                    onChange={this.props.ReportfilterProjectDD}
                    id="select"
                    value={this.props.projectDDSelectedValue}
                    className="form-control"
                  >
                    <option value="">All</option>
                    {TempprojectsList}
                  </select>
                  <label className="modal_label">Project Name</label>
                </div>
              </div>
              {/* <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    id="select"
                    className="form-control"
                    name="ProjectType"
                    onChange={this.props.ReportHandleChange}
                    value={this.props.ShowSelectedDataAdmin.ProjectType}
                  >
                    <option value="All">All</option>
                    <option value="Digital Marketing">Digital Marketing</option>
                    <option value="Software Development">
                      Software Development
                    </option>
                  </select>
                  <label className="modal_label">Project Type</label>
                </div>
              </div> */}
              <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    id="select"
                    name="Department"
                    onChange={this.props.ReportHandleChange}
                    className="form-control"
                    value={this.props.ShowSelectedDataAdmin.Department}
                  >
                    <option value="">All</option>
                    {lstDepartmentNames}
                  </select>
                  <label className="modal_label">Department</label>
                </div>
              </div>
              <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    id="select"
                    name="MainTask"
                    onChange={this.props.ReportHandleChange}
                    className="form-control"
                    value={this.props.ShowSelectedDataAdmin.MainTask}
                  >
                    <option value="">All</option>
                    {TempmainTaskList}
                  </select>
                  <label className="modal_label">Main Task</label>
                </div>
              </div>
              <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    id="select"
                    className="form-control"
                    name="SubTask"
                    onChange={this.props.ReportHandleChange}
                    value={this.props.ShowSelectedDataAdmin.SubTask}
                  >
                    <option value="">All</option>
                    {SubTaskList}
                  </select>
                  <label className="modal_label">Sub Task</label>
                </div>
              </div>
              {/* <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    id="select"
                    className="form-control"
                    name="TaskOwner"
                    onChange={this.props.ReportHandleChange}
                    value={this.props.ShowSelectedDataAdmin.TaskOwner}
                  >
                    <option value="-1">All</option>
                    
                    {TaskOwnerNameList}
                  </select>
                  <label className="modal_label">Task Owner</label>
                </div>
              </div> */}
              <div className="col-md-4">
                <div className="form-group ems_task_owner_selections styled-select ems_task_owner_parent">
                <ReactMultiSelectCheckboxes
                  options={this.props.lstTaskOwnerName_List}
                  placeholderButtonLabel="Select"
                  getDropdownButtonLabel={this.props.getDropdownButtonLabel}
                  value={this.props.setSelectedOptions}
                  onChange={this.props.onchangehandle}
                  setState={this.props.setSelectedOptions}
                />
                     {/* options={ TaskOwnerNameList} />  */}

                   <label className="modal_label ems_task_owner">Task Owner</label>
           </div>
              </div>  

              <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    name="select"
                    onChange={this.props.ReportfilterHourDayDD}
                    id="select"
                    className="form-control"
                  >
                    {this.props.ReportfilterHourDayDDshow !== "" ? (
                      <option value="" disabled selected hidden>
                        {this.props.ReportfilterHourDayDDshow}
                      </option>
                    ) : (
                      <>
                        <option>---Select---</option>
                      </>
                    )}
                    <option>---Select---</option>
                    <option>Hours</option>
                    <option>Days</option>
                  </select>
                  <label className="modal_label">Hours/Days</label>
                </div>
              </div>
              {/* <div className="col-md-4">
                <div className="form-group styled-select">
                  <select
                    name="ExportType"
                    onChange={this.props.SelectExportTypeFun}
                    id="select"
                    className="form-control"
                    value={this.props.ShowSelectedDataAdmin.ExportType}
                  >
                    <option value={"Combine"}>Combine</option>
                    <option value={"Separate"}>Separate</option>
                  </select>
                  <label className="modal_label">Export Type</label>
                </div>
              </div> */}
              <div className="col-md-4">
                <div className="form-group datepicker_mbl">
                  {this.props.showStartDates == false ? (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={"DD/MM/YYYY"}
                        selected={this.props.showStartDates}
                        shouldCloseOnSelect={true}
                        className="form-control"
                        onChange={this.props.StartReportGridDateFormat}
                        highlightDates={[
                          moment().subtract(7, "days"),
                          moment().add(7, "days"),
                        ]}
                      />
                      <label className="datepicker_label">Start Date</label>
                    </div>
                  ) : (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={this.props.showStartDates}
                        selected={this.props.showStartDates}
                        shouldCloseOnSelect={true}
                        className="form-control"
                        onChange={this.props.StartReportGridDateFormat}
                        highlightDates={[
                          moment().subtract(7, "days"),
                          moment().add(7, "days"),
                        ]}
                      />
                      <label className="datepicker_label">Start Date</label>
                    </div>
                  )}
                  {/* <div className="input-group-addon"><i className="fa fa-calendar"></i></div> */}
                </div>
              </div>
              <div className="col-md-4">
                <div className="form-group">
                  {this.props.showEndDates == false ? (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={"DD/MM/YYYY"}
                        selected={this.props.showEndDates}
                        shouldCloseOnSelect={true}
                        className="form-control"
                        onChange={this.props.EndReportGridDateFormat}
                      />
                      <label className="datepicker_label">End Date </label>
                    </div>
                  ) : (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={this.props.showEndDates}
                        selected={this.props.showEndDates}
                        shouldCloseOnSelect={true}
                        className="form-control input-md "
                        onChange={this.props.EndReportGridDateFormat}
                      />
                      <label className="datepicker_label">End Date </label>
                    </div>
                  )}
                  {/*  <div className="input-group-addon"><i className="fa fa-calendar"></i></div> */}
                </div>
              </div>
            </>
          ) : (
            <>
              <div className="col-md-6">
                <div className="form-group">
                  <select
                    name="select"
                    onChange={this.props.ReportfilterProjectDD}
                    id="select"
                    className="form-control"
                    value={this.props.projectDDSelectedValue}
                  >
                    {/* {this.props.projectDDSelectedValueShows !== "" ? (
                      <option value="" disabled selected hidden>
                        {this.props.projectDDSelectedValueShows}
                      </option>
                    ) : (
                      <option value="">All Projects</option>
                    )} */}
                    <option value="">All Projects</option>
                    {TempprojectsList}
                  </select>
                  <label className="col-md-3">Project</label>
                </div>
              </div>

              <div className="col-md-6">
                <div className="form-group">
                  <select
                    name="select"
                    onChange={this.props.ReportfilterHourDayDD}
                    id="select"
                    className="form-control"
                  >
                    {this.props.ReportfilterHourDayDDshow !== "" ? (
                      <option value="" disabled selected hidden>
                        {this.props.ReportfilterHourDayDDshow}
                      </option>
                    ) : (
                      <>
                        <option>---Select---</option>
                      </>
                    )}
                    <option>---Select---</option>
                    <option>Hours</option>
                    <option>Days</option>
                  </select>
                  <label>Hours/Days</label>
                </div>
              </div>
              <div className="col-md-6">
                <div className="form-group datepicker_mbl">
                  {this.props.showStartDates == true ? (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={"DD/MM/YYYY"}
                        selected={this.props.showStartDates}
                        shouldCloseOnSelect={true}
                        className="form-control"
                        onChange={this.props.StartReportGridDateFormat}
                        highlightDates={[
                          moment().subtract(7, "days"),
                          moment().add(7, "days"),
                        ]}
                      />
                      <label className="datepicker_label">Start Date</label>
                    </div>
                  ) : (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={this.props.showStartDates}
                        selected={this.props.showStartDates}
                        shouldCloseOnSelect={true}
                        className="form-control"
                        onChange={this.props.StartReportGridDateFormat}
                        highlightDates={[
                          moment().subtract(7, "days"),
                          moment().add(7, "days"),
                        ]}
                      />
                      <label className="datepicker_label">Start Date</label>
                    </div>
                  )}
                  {/* <div className="input-group-addon"><i className="fa fa-calendar"></i></div> */}
                </div>
              </div>
              <div className="col-md-6">
                <div className="form-group ">
                  {this.props.showEndDates == false ? (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={"DD/MM/YYYY"}
                        selected={this.props.showEndDates}
                        shouldCloseOnSelect={true}
                        className="form-control"
                        onChange={this.props.EndReportGridDateFormat}
                      />
                      <label className="datepicker_label">End Date</label>
                    </div>
                  ) : (
                    <div className="customDatePickerWidth">
                      <DatePicker
                        dateFormat="dd/MM/yyyy"
                        placeholderText={this.props.showEndDates}
                        selected={this.props.showEndDates}
                        shouldCloseOnSelect={true}
                        className="form-control "
                        onChange={this.props.EndReportGridDateFormat}
                      />
                      <label className="datepicker_label">End Date</label>
                    </div>
                  )}
                  {/*  <div className="input-group-addon"><i className="fa fa-calendar"></i></div> */}
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    );
  }
}
export default ReportFilterDesign;
